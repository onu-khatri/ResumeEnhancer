export type RouteShell = 'authenticated' | 'public' | 'transient';

export interface RouteContext {
    shell: RouteShell;
    title: string;
    warnsOnUnsavedWork?: boolean;
}

export const routeContexts = {
    builder: {
        shell: 'authenticated',
        title: 'Resume builder',
        warnsOnUnsavedWork: true,
    },
    dashboard: { shell: 'authenticated', title: 'Resume dashboard' },
    login: { shell: 'public', title: 'Sign in' },
    preview: { shell: 'authenticated', title: 'Resume preview' },
} as const satisfies Record<string, RouteContext>;
