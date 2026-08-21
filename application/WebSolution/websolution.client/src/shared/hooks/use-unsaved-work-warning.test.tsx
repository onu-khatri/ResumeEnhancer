import { render } from '@testing-library/react';
import { describe, expect, it } from 'vitest';

import { useUnsavedWorkWarning } from '@/shared/hooks/use-unsaved-work-warning';

function WarningHarness({ dirty }: { dirty: boolean }) {
    useUnsavedWorkWarning(dirty);
    return null;
}

describe('useUnsavedWorkWarning', () => {
    it('registers and removes the beforeunload warning only for dirty work', () => {
        const { rerender, unmount } = render(<WarningHarness dirty={false} />);
        const cleanEvent = new Event('beforeunload', { cancelable: true });
        expect(window.dispatchEvent(cleanEvent)).toBe(true);
        rerender(<WarningHarness dirty />);
        const dirtyEvent = new Event('beforeunload', { cancelable: true });
        expect(window.dispatchEvent(dirtyEvent)).toBe(false);
        unmount();
        const afterUnmount = new Event('beforeunload', { cancelable: true });
        expect(window.dispatchEvent(afterUnmount)).toBe(true);
    });
});
