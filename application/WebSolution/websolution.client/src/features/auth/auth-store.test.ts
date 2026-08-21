import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';

import { useAuthStore } from '@/features/auth/auth-store';

describe('auth store', () => {
    beforeEach(() => {
        useAuthStore.setState({ isReady: false, session: null });
        document.cookie = 'resume-enhancer-session=; path=/; max-age=0';
    });

    afterEach(() => {
        vi.useRealTimers();
    });

    it('creates, refreshes, updates, and clears a local workspace session', async () => {
        vi.useFakeTimers();
        vi.setSystemTime(new Date('2026-01-01T00:00:00.000Z'));

        useAuthStore.getState().signIn();
        const signedInSession = useAuthStore.getState().session;
        expect(signedInSession).toMatchObject({
            displayName: 'Workspace User',
            resumeId: null,
            userId: 'local-resume-user',
        });
        expect(document.cookie).toContain('resume-enhancer-session=active');

        useAuthStore.getState().updateActiveResumeId(42);
        expect(useAuthStore.getState().session?.resumeId).toBe(42);

        vi.setSystemTime(new Date('2026-01-01T00:10:00.000Z'));
        await expect(useAuthStore.getState().refreshSession()).resolves.toBe(
            true,
        );
        expect(useAuthStore.getState().session?.expiresAt).toBe(
            '2026-01-01T00:40:00.000Z',
        );

        useAuthStore.getState().markReady();
        expect(useAuthStore.getState().isReady).toBe(true);
        useAuthStore.getState().logout();
        expect(useAuthStore.getState().session).toBeNull();
    });

    it('returns false and leaves state unchanged when no session exists', async () => {
        await expect(useAuthStore.getState().refreshSession()).resolves.toBe(
            false,
        );
        useAuthStore.getState().updateActiveResumeId(42);
        expect(useAuthStore.getState().session).toBeNull();
    });
});
