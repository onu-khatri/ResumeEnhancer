import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it, vi } from 'vitest';

import { DashboardPagination } from '@/features/resume/dashboard/components/dashboard-pagination';

describe('DashboardPagination', () => {
    it('forwards API pagination state to the shared pagination control', async () => {
        const user = userEvent.setup();
        const onPageChange = vi.fn();
        render(
            <DashboardPagination
                onPageChange={onPageChange}
                result={{
                    hasNextPage: true,
                    hasPreviousPage: true,
                    items: [],
                    pageNumber: 2,
                    pageSize: 10,
                    totalCount: 25,
                    totalPages: 3,
                }}
            />,
        );
        expect(screen.getByText(/25 total results/)).toBeInTheDocument();
        await user.click(screen.getByRole('button', { name: 'Previous' }));
        await user.click(screen.getByRole('button', { name: 'Next' }));
        expect(onPageChange).toHaveBeenNthCalledWith(1, 1);
        expect(onPageChange).toHaveBeenNthCalledWith(2, 3);
    });
});
