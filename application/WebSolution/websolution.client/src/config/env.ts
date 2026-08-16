const fallbackApiBaseUrl = '/api';

export const env = {
    apiBaseUrl: import.meta.env.VITE_API_BASE_URL ?? fallbackApiBaseUrl,
    draftStorageKey:
        import.meta.env.VITE_RESUME_DRAFT_KEY ?? 'resume-enhancer.resume-draft',
    authStorageKey:
        import.meta.env.VITE_AUTH_STORAGE_KEY ?? 'resume-enhancer.auth-session',
    sessionCookieName:
        import.meta.env.VITE_AUTH_COOKIE_NAME ?? 'resume-enhancer-session',
};
