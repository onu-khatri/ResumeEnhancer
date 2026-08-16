import { create } from 'zustand';
import { createJSONStorage, persist } from 'zustand/middleware';

import { env } from '@/config/env';

export interface AuthSession {
    accessToken: string | null;
    displayName: string;
    email: string;
    expiresAt: string;
    refreshToken: string | null;
    resumeId: number | null;
    userId: string;
}

interface AuthStoreState {
    isReady: boolean;
    session: AuthSession | null;
}

interface AuthStoreActions {
    logout: () => void;
    markReady: () => void;
    refreshSession: () => Promise<boolean>;
    signIn: () => void;
    updateActiveResumeId: (resumeId: number | null) => void;
}

type AuthStore = AuthStoreState & AuthStoreActions;

const SESSION_DURATION_MS = 1000 * 60 * 30;

function createExpiryDate() {
    return new Date(Date.now() + SESSION_DURATION_MS).toISOString();
}

function syncSessionCookie(hasSession: boolean) {
    if (typeof document === 'undefined') {
        return;
    }

    document.cookie = hasSession
        ? `${env.sessionCookieName}=active; path=/; max-age=${SESSION_DURATION_MS / 1000}; samesite=lax`
        : `${env.sessionCookieName}=; path=/; max-age=0; samesite=lax`;
}

function createLocalSession(): AuthSession {
    return {
        accessToken: null,
        displayName: 'Workspace User',
        email: 'workspace.user@local.dev',
        expiresAt: createExpiryDate(),
        refreshToken: null,
        resumeId: null,
        userId: 'local-resume-user',
    };
}

function isSessionExpired(session: AuthSession | null) {
    return session
        ? new Date(session.expiresAt).getTime() <= Date.now()
        : false;
}

export const useAuthStore = create<AuthStore>()(
    persist(
        (set, get) => ({
            isReady: false,
            logout: () => {
                syncSessionCookie(false);
                set({ session: null });
            },
            markReady: () => set({ isReady: true }),
            refreshSession: async () => {
                const currentSession = get().session;
                if (!currentSession) {
                    return false;
                }

                const nextSession = {
                    ...currentSession,
                    expiresAt: createExpiryDate(),
                };

                syncSessionCookie(true);
                set({ session: nextSession });
                return true;
            },
            session: null,
            signIn: () => {
                const session = createLocalSession();
                syncSessionCookie(true);
                set({ session });
            },
            updateActiveResumeId: (resumeId) => {
                const currentSession = get().session;
                if (!currentSession) {
                    return;
                }

                syncSessionCookie(true);
                set({
                    session: {
                        ...currentSession,
                        resumeId,
                    },
                });
            },
        }),
        {
            merge: (persistedState, currentState) => {
                const nextState = persistedState as
                    Partial<AuthStoreState> | undefined;
                const nextSession = nextState?.session ?? null;

                if (isSessionExpired(nextSession)) {
                    syncSessionCookie(false);
                    return { ...currentState, isReady: true, session: null };
                }

                syncSessionCookie(nextSession !== null);
                return {
                    ...currentState,
                    ...nextState,
                    isReady: true,
                    session: nextSession,
                };
            },
            name: env.authStorageKey,
            onRehydrateStorage: () => (state) => {
                if (!state) {
                    syncSessionCookie(false);
                    return;
                }

                if (isSessionExpired(state.session)) {
                    syncSessionCookie(false);
                    state.logout();
                    state.markReady();
                    return;
                }

                syncSessionCookie(state.session !== null);
                state.markReady();
            },
            partialize: (state) => ({
                session: state.session,
            }),
            storage: createJSONStorage(() => localStorage),
        },
    ),
);
