import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it, vi } from 'vitest';

import {
    EmptyState,
    ErrorState,
    InlineAlert,
    SkeletonBlock,
} from '@/shared/ui/status';

describe('status surfaces', () => {
    it('renders alerts, empty states, skeletons, and retryable errors', async () => {
        const user = userEvent.setup();
        const onRetry = vi.fn();
        const { rerender } = render(
            <InlineAlert message="Saved" title="Success" />,
        );
        expect(screen.getByText('Saved')).toBeInTheDocument();
        rerender(
            <EmptyState
                action={<button type="button">Create</button>}
                description="None"
                title="Empty"
            />,
        );
        expect(
            screen.getByRole('button', { name: 'Create' }),
        ).toBeInTheDocument();
        rerender(<ErrorState description="Failed" onRetry={onRetry} />);
        await user.click(screen.getByRole('button', { name: 'Try again' }));
        expect(onRetry).toHaveBeenCalledOnce();
        rerender(<SkeletonBlock className="h-4" />);
        expect(document.querySelector('.h-4')).toBeInTheDocument();
    });

    it('renders errors without retry', () => {
        render(<ErrorState description="Failed" />);
        expect(
            screen.queryByRole('button', { name: 'Try again' }),
        ).not.toBeInTheDocument();
    });
});
