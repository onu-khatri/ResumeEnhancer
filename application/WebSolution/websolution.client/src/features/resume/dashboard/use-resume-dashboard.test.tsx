import { act, renderHook } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';

const mocks = vi.hoisted(() => ({
    navigate: vi.fn(),
    search: vi.fn(),
    updateActiveResumeId: vi.fn(),
}));

vi.mock('react-router-dom', () => ({
    useNavigate: () => mocks.navigate,
}));
vi.mock('@/features/auth/auth-context', () => ({
    useAuth: () => ({
        session: { userId: 'user-1' },
        updateActiveResumeId: mocks.updateActiveResumeId,
    }),
}));
vi.mock('@/features/resume/hooks/use-resume-search', () => ({
    useResumeSearch: (request: unknown) => {
        mocks.search(request);
        return { data: undefined };
    },
}));
vi.mock('@/features/resume/hooks/use-resume-delete', () => ({
    useResumeDelete: () => ({ mutateAsync: vi.fn() }),
}));

import { useResumeDashboard } from '@/features/resume/dashboard/use-resume-dashboard';

describe('useResumeDashboard', () => {
    it('builds a trimmed search request and routes selection actions', () => {
        const { result } = renderHook(() => useResumeDashboard());

        expect(mocks.search).toHaveBeenLastCalledWith({
            pageNumber: 1,
            pageSize: 12,
            searchText: null,
            sortBy: 2,
            sortDirection: 1,
            userId: 'user-1',
        });
        act(() => result.current.setSearchText(' engineer '));
        act(() => result.current.setPageNumber(2));
        expect(mocks.search).toHaveBeenLastCalledWith({
            pageNumber: 2,
            pageSize: 12,
            searchText: 'engineer',
            sortBy: 2,
            sortDirection: 1,
            userId: 'user-1',
        });

        act(() => result.current.createResume());
        act(() => result.current.selectResume(9, 'preview'));
        expect(mocks.updateActiveResumeId).toHaveBeenNthCalledWith(1, null);
        expect(mocks.updateActiveResumeId).toHaveBeenNthCalledWith(2, 9);
        expect(mocks.navigate).toHaveBeenNthCalledWith(
            1,
            '/app/resume/builder',
        );
        expect(mocks.navigate).toHaveBeenNthCalledWith(
            2,
            '/app/resume/preview',
        );
    });
});
