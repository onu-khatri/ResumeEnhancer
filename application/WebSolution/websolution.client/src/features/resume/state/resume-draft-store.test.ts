import { beforeEach, describe, expect, it, vi } from 'vitest';

import { createEmptyResumeForm } from '@/features/resume/model/types';
import { useResumeDraftStore } from '@/features/resume/state/resume-draft-store';

describe('resume draft store', () => {
    beforeEach(() => {
        useResumeDraftStore.setState({ draft: null });
    });

    it('saves a timestamped draft and clears it', () => {
        vi.useFakeTimers();
        vi.setSystemTime(new Date('2026-02-01T12:00:00.000Z'));
        const values = createEmptyResumeForm('user-1');

        const updatedAt = useResumeDraftStore.getState().saveDraft(values, 7);

        expect(updatedAt).toBe('2026-02-01T12:00:00.000Z');
        expect(useResumeDraftStore.getState().draft).toEqual({
            resumeId: 7,
            updatedAt,
            values,
        });

        useResumeDraftStore.getState().clearDraft();
        expect(useResumeDraftStore.getState().draft).toBeNull();
        vi.useRealTimers();
    });
});
