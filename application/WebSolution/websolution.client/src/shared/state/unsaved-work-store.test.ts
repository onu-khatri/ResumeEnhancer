import { describe, expect, it } from 'vitest';

import { useUnsavedWorkStore } from '@/shared/state/unsaved-work-store';

describe('unsaved work store', () => {
    it('tracks the current unsaved-work state', () => {
        useUnsavedWorkStore.getState().setHasUnsavedWork(true);
        expect(useUnsavedWorkStore.getState().hasUnsavedWork).toBe(true);
        useUnsavedWorkStore.getState().setHasUnsavedWork(false);
        expect(useUnsavedWorkStore.getState().hasUnsavedWork).toBe(false);
    });
});
