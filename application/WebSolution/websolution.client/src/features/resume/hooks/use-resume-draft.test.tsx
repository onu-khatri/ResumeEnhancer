import { act, renderHook } from '@testing-library/react';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';

import { useResumeDraft } from '@/features/resume/hooks/use-resume-draft';
import { createEmptyResumeForm } from '@/features/resume/model/types';
import { useResumeDraftStore } from '@/features/resume/state/resume-draft-store';

describe('useResumeDraft', () => {
    beforeEach(() => {
        useResumeDraftStore.setState({ draft: null });
        vi.useFakeTimers();
    });

    afterEach(() => {
        vi.useRealTimers();
    });

    it('debounces enabled drafts and clears the displayed timestamp', () => {
        vi.setSystemTime(new Date('2026-02-01T12:00:00.000Z'));
        const values = createEmptyResumeForm('user-1');
        const { result } = renderHook(() => useResumeDraft(values, 4, true));

        act(() => vi.advanceTimersByTime(499));
        expect(useResumeDraftStore.getState().draft).toBeNull();
        act(() => vi.advanceTimersByTime(1));

        expect(result.current.lastSavedAt).toBe('2026-02-01T12:00:00.500Z');
        expect(useResumeDraftStore.getState().draft?.resumeId).toBe(4);

        act(() => result.current.clearDraft());
        expect(result.current.lastSavedAt).toBeNull();
        expect(useResumeDraftStore.getState().draft).toBeNull();
    });

    it('does not schedule a draft while disabled', () => {
        const values = createEmptyResumeForm('user-1');
        renderHook(() => useResumeDraft(values, null, false));

        act(() => vi.advanceTimersByTime(500));
        expect(useResumeDraftStore.getState().draft).toBeNull();
    });
});
