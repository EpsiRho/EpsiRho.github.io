import {BetaClient, EnrollmentCookies, packageId} from './betas-api.js';

const $ = selector => document.querySelector(selector);
const client = new BetaClient(window.BETA_DEN_CONFIG);
const cookies = new EnrollmentCookies();
const enrollments = cookies.load();
const downloadsDialog = $('#downloads-dialog');
const feedbackDialog = $('#feedback-dialog');
let enrolling = false;
let restoring = true;
let cooldownTimer;
let currentDownload = null;
let currentFeedback = null;
let downloadGeneration = 0;
let sendingFeedback = false;
let feedbackDraftId = null;
let cookieWarning = false;

function element(tag, className, text) {
    const node = document.createElement(tag);
    if (className) node.className = className;
    if (text !== undefined) node.textContent = text;
    return node;
}
function message(target, text = '', state = '') {
    target.textContent = text;
    target.className = `form-message ${state}`;
}
function toast(text) {
    const item = element('div', 'toast');
    const close = element('button', '', '×');
    close.type = 'button';
    close.setAttribute('aria-label', 'Dismiss notification');
    close.addEventListener('click', () => item.remove());
    item.append(element('p', '', text), close);
    $('#toasts').append(item);
    // Expiration notices remain available until dismissed, including on mobile.
}
function save(entry) {
    try { cookies.save(entry); }
    catch {
        if (!cookieWarning) {
            cookieWarning = true;
            toast('Your browser could not save this enrollment. You can use it now, but may need to enter your code again next time.');
        }
    }
}
function expire(entry) {
    const index = enrollments.findIndex(item => item.id === entry.id);
    if (index === -1) return;
    enrollments.splice(index, 1);
    cookies.remove(entry);
    if (currentDownload?.id === entry.id) downloadsDialog.close();
    if (currentFeedback?.id === entry.id) feedbackDialog.close();
    toast(`${entry.name || 'Your saved beta'} has expired. Its enrollment has been removed from this browser.`);
    render();
}
function errorText(error) {
    return error?.message && error?.kind ? error.message : 'Something went wrong while loading the server response. Please try again.';
}
function handleError(error, entry, target) {
    if (['invalid', 'expired'].includes(error.kind) && entry) { expire(entry); return; }
    message(target, errorText(error), 'error');
    if (error.kind === 'rate') startCooldown();
}
function updateEnrollButton() {
    const remaining = Math.max(0, Math.ceil((client.retryAt - Date.now()) / 1000));
    const button = $('#enroll-submit');
    button.disabled = enrolling || restoring || remaining > 0;
    button.textContent = enrolling ? 'Enrolling…' : remaining ? `Try again in ${Math.floor(remaining / 60)}:${String(remaining % 60).padStart(2, '0')}` : 'Enroll';
    if (!remaining && cooldownTimer) { clearInterval(cooldownTimer); cooldownTimer = null; }
}
function startCooldown() {
    updateEnrollButton();
    if (!cooldownTimer) cooldownTimer = setInterval(updateEnrollButton, 1000);
}
function action(label, callback, primary = false) {
    const button = element('button', `beta-button${primary ? ' beta-primary' : ''}`, label);
    button.type = 'button';
    button.addEventListener('click', callback);
    return button;
}
function render() {
    const list = $('#enrollments');
    list.replaceChildren();
    list.setAttribute('aria-busy', String(restoring));
    $('#enrollment-count').textContent = String(enrollments.length);
    if (!enrollments.length) {
        const empty = element('div', 'empty-state');
        empty.append(element('strong', '', 'No betas enrolled yet.'), element('p', '', 'Enter an invite code above to get started.'));
        list.append(empty);
        return;
    }
    for (const entry of enrollments) {
        const card = element('article', 'enrollment');
        const header = element('div', 'enrollment-header');
        header.append(element('h3', '', entry.info?.name || entry.name || 'Saved beta'));
        if (entry.info?.latestVersion) header.append(element('span', 'enrollment-version', `Latest: ${entry.info.latestVersion}`));
        card.append(header);
        if (entry.info?.description) card.append(element('p', 'enrollment-description', entry.info.description));
        if (entry.loading) card.append(element('p', 'muted', 'Refreshing your enrollment…'));
        if (entry.error) card.append(element('p', 'form-message error', entry.error));
        const buttons = element('div', 'enrollment-actions');
        const downloads = action('Downloads', () => openDownloads(entry), true);
        const feedback = action('Send feedback', () => openFeedback(entry));
        downloads.disabled = feedback.disabled = !!entry.loading;
        buttons.append(downloads, feedback);
        if (entry.error) buttons.append(action('Retry', async event => {
            event.currentTarget.disabled = true;
            await refreshInfo(entry);
        }));
        card.append(buttons);
        list.append(card);
    }
}
async function refreshInfo(entry) {
    entry.loading = true;
    entry.error = '';
    render();
    try {
        entry.info = await client.info(entry);
        entry.name = entry.info.name;
        save(entry);
    } catch (error) {
        if (['invalid', 'expired'].includes(error.kind)) expire(entry);
        else {
            entry.error = errorText(error);
            if (error.kind === 'rate') startCooldown();
        }
    } finally { entry.loading = false; render(); }
}

$('#enroll-form').addEventListener('submit', async event => {
    event.preventDefault();
    if (enrolling || restoring || client.retryAt > Date.now()) return;
    const input = $('#invite-code'), code = input.value.trim();
    input.setCustomValidity(code ? '' : 'Enter your invite code.');
    if (!$('#enroll-form').reportValidity()) return;
    enrolling = true;
    updateEnrollButton();
    message($('#enroll-message'), 'Checking your invite code…');
    try {
        let entry = enrollments.find(item => item.code === code);
        if (entry) {
            await refreshInfo(entry);
            if (!enrollments.includes(entry)) {
                message($('#enroll-message'), 'This invite code may be invalid, or the beta may have already ended.', 'error');
                return;
            }
            if (entry.error) { message($('#enroll-message'), entry.error, 'error'); return; }
            message($('#enroll-message'), `You’re already enrolled in ${entry.name}.`, 'success');
        } else {
            const token = await client.validate(code);
            const id = packageId(token);
            const existing = id && enrollments.find(item => item.packageId === id);
            entry = existing || {id: crypto.randomUUID(), code, token, packageId: id, name: 'Your beta'};
            Object.assign(entry, {code, token, packageId: id});
            if (!existing) enrollments.push(entry);
            save(entry);
            await refreshInfo(entry);
            if (!enrollments.includes(entry)) {
                message($('#enroll-message'), 'This invite code may be invalid, or the beta may have already ended.', 'error');
                return;
            }
            message($('#enroll-message'), entry.error ? 'Your code was accepted. Package details could not be loaded; use Retry below.' : existing ? `You’re already enrolled in ${entry.name}.` : `You’re enrolled in ${entry.name}.`, entry.error ? 'error' : 'success');
        }
        input.value = '';
    } catch (error) { handleError(error, null, $('#enroll-message')); }
    finally { enrolling = false; updateEnrollButton(); render(); }
});
$('#invite-code').addEventListener('input', event => event.target.setCustomValidity(''));

async function openDownloads(entry) {
    currentDownload = entry;
    const generation = ++downloadGeneration;
    $('#downloads-package').textContent = entry.name;
    const list = $('#versions');
    list.replaceChildren(element('p', 'muted', 'Loading versions…'));
    list.setAttribute('aria-busy', 'true');
    message($('#download-message'));
    if (!downloadsDialog.open) downloadsDialog.showModal();
    $('#downloads-title').focus();
    try {
        const versions = await client.versions(entry);
        if (generation !== downloadGeneration || !downloadsDialog.open) return;
        list.replaceChildren();
        if (!versions.length) list.append(element('p', 'empty-state', 'No downloads are available for this beta yet.'));
        versions.forEach((version, index) => {
            const item = element('article', 'version-item');
            const heading = element('div', 'version-heading');
            const title = element('h3');
            const button = element('button', 'version-download', version.version);
            button.type = 'button';
            button.setAttribute('aria-label', `Download version ${version.version}`);
            button.addEventListener('click', () => downloadVersion(entry, version, button, generation));
            title.append(button);
            heading.append(title);
            if (!index) heading.append(element('span', 'version-latest', 'Latest'));
            item.append(heading, element('p', 'version-description', version.description || 'No changelog provided.'));
            list.append(item);
        });
    } catch (error) {
        if (generation !== downloadGeneration) return;
        list.replaceChildren(action('Retry loading versions', () => openDownloads(entry)));
        handleError(error, entry, $('#download-message'));
    } finally { if (generation === downloadGeneration) list.setAttribute('aria-busy', 'false'); }
}
async function downloadVersion(entry, version, button, generation) {
    button.disabled = true;
    message($('#download-message'), `Preparing version ${version.version}…`);
    try {
        const response = await client.download(entry, version.downloadId);
        const blob = await response.blob();
        const href = URL.createObjectURL(blob);
        const anchor = document.createElement('a');
        anchor.href = href;
        // The API currently emits the BetaVersion class name in Content-Disposition;
        // use the known package/version and ZIP content type for a useful filename.
        anchor.download = `${entry.name} v${version.version}.zip`.replace(/[<>:"/\\|?*\x00-\x1f]/g, '_');
        document.body.append(anchor);
        anchor.click();
        anchor.remove();
        setTimeout(() => URL.revokeObjectURL(href), 60000);
        if (generation === downloadGeneration) message($('#download-message'), `Version ${version.version} is ready. Check your browser’s downloads.`, 'success');
    } catch (error) {
        if (generation === downloadGeneration) handleError(error, entry, $('#download-message'));
        else if (error.kind === 'invalid') expire(entry);
    } finally { button.disabled = false; }
}
function openFeedback(entry) {
    currentFeedback = entry;
    if (feedbackDraftId !== entry.id) {
        $('#feedback-form').reset();
        $('#feedback-form').querySelectorAll('input, textarea').forEach(input => input.setCustomValidity(''));
        feedbackDraftId = entry.id;
    }
    $('#feedback-package').textContent = entry.name;
    message($('#feedback-message'));
    feedbackDialog.showModal();
    $('#issue-title').focus();
}
function feedbackBusy(busy) {
    sendingFeedback = busy;
    feedbackDialog.querySelectorAll('input, textarea, select, button').forEach(control => { control.disabled = busy; });
    $('#feedback-submit').textContent = busy ? 'Sending…' : 'Send feedback';
}
$('#feedback-form').addEventListener('submit', async event => {
    event.preventDefault();
    if (sendingFeedback || !currentFeedback) return;
    const form = event.currentTarget;
    for (const id of ['issue-title', 'issue-description']) {
        const input = document.getElementById(id);
        input.setCustomValidity(input.value.trim() ? '' : 'Please fill out this field.');
    }
    if (!form.reportValidity()) return;
    const fields = Object.fromEntries(Array.from(new FormData(form), ([key, value]) => [key, value.trim()]));
    feedbackBusy(true);
    message($('#feedback-message'), 'Sending your feedback…');
    const entry = currentFeedback;
    try {
        await client.feedback(entry, fields);
        form.reset();
        feedbackDraftId = null;
        feedbackDialog.close();
        toast(`Thanks! Your feedback for ${entry.name} has been sent.`);
    } catch (error) { handleError(error, entry, $('#feedback-message')); }
    finally { feedbackBusy(false); }
});
$('#feedback-form').addEventListener('input', event => { if (event.target.setCustomValidity) event.target.setCustomValidity(''); });

document.querySelectorAll('[data-close-dialog]').forEach(button => button.addEventListener('click', () => button.closest('dialog').close()));
for (const dialog of [downloadsDialog, feedbackDialog]) {
    let backdropPointer = false;
    dialog.addEventListener('pointerdown', event => { backdropPointer = event.target === dialog; });
    dialog.addEventListener('click', event => {
        const box = dialog.getBoundingClientRect();
        const outside = event.clientX < box.left || event.clientX > box.right || event.clientY < box.top || event.clientY > box.bottom;
        if (backdropPointer && event.target === dialog && outside && !(dialog === feedbackDialog && sendingFeedback)) dialog.close();
    });
    dialog.addEventListener('close', () => {
        if (dialog === downloadsDialog) { downloadGeneration++; currentDownload = null; }
        if (document.activeElement === document.body) $('#invite-code').focus();
    });
}
feedbackDialog.addEventListener('cancel', event => { if (sendingFeedback) event.preventDefault(); });

async function restore() {
    updateEnrollButton();
    render();
    for (const entry of [...enrollments]) await refreshInfo(entry);
    restoring = false;
    updateEnrollButton();
    render();
}
restore();
