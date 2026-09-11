// All authorization is enforced by the API. JWT claims here are scheduling and
// display hints only; they never grant access to a package or a download.
export class BetaError extends Error {
    constructor(kind, message, retryAt = 0) {
        super(message);
        this.kind = kind;
        this.retryAt = retryAt;
    }
}

export function tokenClaims(token) {
    try {
        const part = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
        return JSON.parse(new TextDecoder().decode(Uint8Array.from(atob(part), c => c.charCodeAt(0))));
    } catch { return {}; }
}

export function packageId(token) {
    const claims = tokenClaims(token);
    const key = Object.keys(claims).find(key => key.toLowerCase() === 'userdata' || key.toLowerCase().endsWith('/userdata'));
    return String(claims.packageId || claims[key] || '');
}

// Numeric components, then SemVer prerelease precedence. Build metadata does
// not change precedence; this also handles the API's four-part .NET versions.
export function compareVersions(a, b) {
    const parse = value => String(value).replace(/^v/i, '').split('+')[0].split(/-(.*)/s);
    const [ac, ap] = parse(a.version), [bc, bp] = parse(b.version);
    const an = ac.split('.'), bn = bc.split('.');
    for (let i = 0; i < Math.max(an.length, bn.length); i++) {
        const av = an[i] || '0', bv = bn[i] || '0';
        const difference = /^\d+$/.test(av) && /^\d+$/.test(bv)
            ? (BigInt(bv) > BigInt(av) ? 1 : BigInt(bv) < BigInt(av) ? -1 : 0)
            : bv.localeCompare(av, undefined, {numeric: true});
        if (difference) return difference;
    }
    if (ap === undefined && bp !== undefined) return -1;
    if (ap !== undefined && bp === undefined) return 1;
    const aa = (ap || '').split('.'), ba = (bp || '').split('.');
    for (let i = 0; i < Math.max(aa.length, ba.length); i++) {
        if (aa[i] === undefined) return 1;
        if (ba[i] === undefined) return -1;
        if (aa[i] === ba[i]) continue;
        const ai = /^\d+$/.test(aa[i]), bi = /^\d+$/.test(ba[i]);
        if (ai && bi) return BigInt(ba[i]) > BigInt(aa[i]) ? 1 : -1;
        if (ai !== bi) return ai ? 1 : -1;
        return ba[i] > aa[i] ? 1 : -1;
    }
    return 0;
}

export function normalizeInfo(data) {
    const info = {name: data?.name ?? data?.Name, description: data?.description ?? data?.Description ?? '', latestVersion: data?.latestVersion ?? data?.LatestVersion ?? ''};
    if (typeof info.name !== 'string' || !info.name || ['Invalid ID', 'Error'].includes(info.name)) {
        throw new BetaError('response', 'Package details are unavailable. Please try again later.');
    }
    return info;
}

export function normalizeVersions(data) {
    if (!Array.isArray(data)) throw new BetaError('response', 'The server returned an unexpected versions list.');
    return data.map(item => ({version: item.version ?? item.Version, description: item.description ?? item.Description ?? '', downloadId: item.downloadID ?? item.downloadId ?? item.DownloadID}))
        .filter(item => typeof item.version === 'string' && typeof item.downloadId === 'string' && item.downloadId)
        .sort(compareVersions);
}

const CODE_PREFIX = 'beta_den_enrollment_v1_';
const TOKEN_PREFIX = 'beta_den_token_v1_';
export class EnrollmentCookies {
    constructor(doc = document, secure = location.protocol === 'https:') { this.doc = doc; this.secure = secure; }
    all() {
        return new Map(this.doc.cookie.split(';').map(cookie => {
            const index = cookie.indexOf('=');
            return [cookie.slice(0, index).trim(), cookie.slice(index + 1)];
        }));
    }
    write(name, value, maxAge) {
        const encoded = encodeURIComponent(value);
        if (name.length + encoded.length > 3800) throw new Error('Cookie too large');
        this.doc.cookie = `${name}=${encoded}; Path=/betas; Max-Age=${Math.max(0, Math.floor(maxAge))}; SameSite=Strict${this.secure ? '; Secure' : ''}`;
        if (maxAge > 0 && this.all().get(name) !== encoded) throw new Error('Cookie storage unavailable');
    }
    load() {
        const cookies = this.all(), entries = [];
        for (const [name, value] of cookies) {
            if (!name.startsWith(CODE_PREFIX)) continue;
            const id = name.slice(CODE_PREFIX.length);
            if (!/^[a-zA-Z0-9-]+$/.test(id)) continue;
            try {
                const record = JSON.parse(decodeURIComponent(value));
                if (typeof record.code !== 'string' || !record.code || record.code.length > 256) throw new Error('Invalid record');
                let token = '';
                try { token = decodeURIComponent(cookies.get(TOKEN_PREFIX + id) || ''); } catch { /* Refresh with the saved code. */ }
                entries.push({id, code: record.code, packageId: record.packageId || '', name: record.name || 'Saved beta', token});
            } catch { this.remove({id}); }
        }
        return entries;
    }
    save(entry) {
        // Keep each enrollment separate to avoid the ~4KB per-cookie limit.
        this.write(CODE_PREFIX + entry.id, JSON.stringify({code: entry.code, packageId: entry.packageId, name: entry.name}), 365 * 86400);
        const remaining = Number(tokenClaims(entry.token).exp) - Date.now() / 1000;
        this.write(TOKEN_PREFIX + entry.id, entry.token || '', Number.isFinite(remaining) ? Math.min(1800, remaining) : 0);
    }
    remove(entry) {
        this.write(CODE_PREFIX + entry.id, '', 0);
        this.write(TOKEN_PREFIX + entry.id, '', 0);
    }
}

export class BetaClient {
    constructor(config, hostname = location.hostname, fetcher = fetch) {
        this.config = config;
        this.base = ['localhost', '127.0.0.1', '[::1]'].includes(hostname) ? config.developmentApiBaseUrl : config.productionApiBaseUrl;
        // Native browser fetch requires the Window receiver when called as a method.
        this.fetcher = fetcher.bind(globalThis);
        this.retryAt = 0;
        this.refreshes = new Map();
        this.validationPath = null;
        this.onRefresh = () => {};
    }
    async request(path, {token, params = {}, method = 'GET', body} = {}) {
        const url = new URL(path, this.base);
        for (const [name, value] of Object.entries(params)) url.searchParams.set(name, value);
        const controller = new AbortController();
        const timer = setTimeout(() => controller.abort(), 30000);
        try {
            const headers = {Accept: path.endsWith('/download') ? 'application/zip' : 'application/json, text/plain'};
            if (token) headers.Authorization = `Bearer ${token}`;
            if (body !== undefined) headers['Content-Type'] = 'application/json';
            const response = await this.fetcher(url, {method, headers, body: body === undefined ? undefined : JSON.stringify(body), credentials: 'omit', cache: 'no-store', referrerPolicy: 'no-referrer', signal: controller.signal});
            if (response.status === 429) {
                const retry = response.headers.get('Retry-After');
                const seconds = retry === null ? NaN : Number(retry);
                const date = retry === null ? NaN : Date.parse(retry);
                this.retryAt = Date.now() + (Number.isFinite(seconds) ? Math.max(1, seconds) * 1000 : Number.isFinite(date) ? Math.max(1000, date - Date.now()) : 300000);
                throw new BetaError('rate', 'The limit of 5 validation requests in 5 minutes has been reached. Please try again when the countdown ends.', this.retryAt);
            }
            if (response.status === 401 || response.status === 403) throw new BetaError('unauthorized', 'Your session needs to be refreshed.');
            if (!response.ok) throw new BetaError(String(response.status), response.status === 404 ? 'This item is no longer available.' : 'The server could not complete the request. Please try again later.');
            return response;
        } catch (error) {
            if (error instanceof BetaError) throw error;
            throw new BetaError('network', 'Unable to reach the beta server. Check your connection and try again.');
        } finally { clearTimeout(timer); }
    }
    async validate(code) {
        if (this.retryAt > Date.now()) throw new BetaError('rate', 'The limit of 5 validation requests in 5 minutes has been reached. Please wait before trying again.', this.retryAt);
        const paths = this.validationPath ? [this.validationPath] : this.config.validationPaths;
        for (let index = 0; index < paths.length; index++) {
            try {
                const response = await this.request(paths[index], {params: {code}});
                this.validationPath = paths[index];
                let token = (await response.text()).trim();
                try { token = JSON.parse(token); } catch { /* ASP.NET also returns plain text. */ }
                if (typeof token !== 'string') throw new BetaError('response', 'The beta server returned an unexpected response. Please try again later.');
                if (/^invalid code\.?$/i.test(token)) throw new BetaError('invalid', 'This invite code may be invalid, or the beta may have already ended.');
                if (token.split('.').length !== 3 || !Number.isFinite(tokenClaims(token).exp)) throw new BetaError('response', 'The beta server returned an unexpected response. Please try again later.');
                return token;
            } catch (error) {
                if (error.kind === '404' && index + 1 < paths.length) continue;
                if (['unauthorized', '400'].includes(error.kind)) throw new BetaError('invalid', 'This invite code may be invalid, or the beta may have already ended.');
                throw error;
            }
        }
    }
    async refresh(entry) {
        if (!this.refreshes.has(entry.id)) {
            this.refreshes.set(entry.id, this.validate(entry.code).then(token => {
                entry.token = token;
                entry.packageId = packageId(token) || entry.packageId;
                this.onRefresh(entry);
            }).finally(() => this.refreshes.delete(entry.id)));
        }
        return this.refreshes.get(entry.id);
    }
    async authorized(entry, path, options = {}) {
        if (!(Number(tokenClaims(entry.token).exp) > Date.now() / 1000 + 30)) await this.refresh(entry);
        try { return await this.request(path, {...options, token: entry.token}); }
        catch (error) {
            if (error.kind !== 'unauthorized') throw error;
            await this.refresh(entry);
            return this.request(path, {...options, token: entry.token});
        }
    }
    async info(entry) { return normalizeInfo(await (await this.authorized(entry, '/api/package/info')).json()); }
    async versions(entry) { return normalizeVersions(await (await this.authorized(entry, '/api/package/versions')).json()); }
    async download(entry, downloadId) { return this.authorized(entry, '/api/package/download', {params: {downloadId}}); }
    async feedback(entry, fields) {
        const method = this.config.feedbackMethod;
        return this.authorized(entry, this.config.feedbackPath, {method, ...(method === 'GET' ? {params: fields} : {body: fields})});
    }
}
