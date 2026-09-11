/* GitHub Pages serves these files directly; no build or server-side state is needed.
 * Local previews use the development API. Public pages use the production API.
 * Set productionApiBaseUrl to your HTTPS API origin when deploying.
 */
window.BETA_DEN_CONFIG = Object.freeze({
    productionApiBaseUrl: 'https://beta.epsirho.com',
    developmentApiBaseUrl: 'http://localhost:5173',
    // The current backend exposes /auth/validate. The first path also supports
    // the intended /api/auth/validate route; fallback occurs only on HTTP 404.
    validationPaths: ['/api/auth/validate', '/auth/validate'],
    feedbackPath: '/api/feedback/send',
    feedbackMethod: 'POST'
});
