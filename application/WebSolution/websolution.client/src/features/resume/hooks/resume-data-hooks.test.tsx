import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { renderHook, waitFor } from '@testing-library/react';
import type { PropsWithChildren } from 'react';
import { describe, expect, it, vi } from 'vitest';

const mocks = vi.hoisted(() => ({
    createResume: vi.fn(),
    deleteResumes: vi.fn(),
    getResume: vi.fn(),
    searchResumes: vi.fn(),
    updateActiveResumeId: vi.fn(),
    updateResume: vi.fn(),
}));

vi.mock('@/features/auth/auth-context', () => ({
    useAuth: () => ({
        logout: vi.fn(),
        refreshSession: vi.fn(),
        session: { accessToken: 'token', resumeId: 2 },
        updateActiveResumeId: mocks.updateActiveResumeId,
    }),
}));

vi.mock('@/shared/api/api-client', () => ({
    createApiClient: vi.fn(() => ({})),
}));

vi.mock('@/features/resume/api/resume-service', () => ({
    createResumeService: () => ({
        createResume: mocks.createResume,
        deleteResumes: mocks.deleteResumes,
        getResume: mocks.getResume,
        searchResumes: mocks.searchResumes,
        updateResume: mocks.updateResume,
    }),
}));

import { useResumeDelete } from '@/features/resume/hooks/use-resume-delete';
import { useResumeQuery } from '@/features/resume/hooks/use-resume-query';
import { useResumeSearch } from '@/features/resume/hooks/use-resume-search';
import { useResumeSubmit } from '@/features/resume/hooks/use-resume-submit';
import { createEmptyResumeForm } from '@/features/resume/model/types';

function createWrapper(queryClient: QueryClient) {
    return function Wrapper({ children }: PropsWithChildren) {
        return (
            <QueryClientProvider client={queryClient}>
                {children}
            </QueryClientProvider>
        );
    };
}

describe('resume data hooks', () => {
    it('disables a null resume query and loads a selected resume', async () => {
        const queryClient = new QueryClient({
            defaultOptions: { queries: { retry: false } },
        });
        mocks.getResume.mockResolvedValue({ id: 2, title: 'Resume' });

        const { result, rerender } = renderHook(
            ({ resumeId }) => useResumeQuery(resumeId),
            {
                initialProps: { resumeId: null as number | null },
                wrapper: createWrapper(queryClient),
            },
        );
        expect(mocks.getResume).not.toHaveBeenCalled();

        rerender({ resumeId: 2 });
        await waitFor(() =>
            expect(result.current.data).toEqual({ id: 2, title: 'Resume' }),
        );
        expect(mocks.getResume).toHaveBeenCalledWith(2);
    });

    it('searches using the supplied request', async () => {
        const queryClient = new QueryClient({
            defaultOptions: { queries: { retry: false } },
        });
        const request = { pageNumber: 1, pageSize: 10, searchText: 'engineer' };
        mocks.searchResumes.mockResolvedValue({ items: [], totalCount: 0 });
        const { result } = renderHook(() => useResumeSearch(request), {
            wrapper: createWrapper(queryClient),
        });

        await waitFor(() => expect(result.current.isSuccess).toBe(true));
        expect(mocks.searchResumes).toHaveBeenCalledWith(request);
    });

    it('creates or updates resumes and invalidates the affected cache', async () => {
        const queryClient = new QueryClient({
            defaultOptions: { mutations: { retry: false } },
        });
        const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');
        mocks.createResume.mockResolvedValue({ id: 5 });
        mocks.updateResume.mockResolvedValue({ id: 8 });
        const { result } = renderHook(() => useResumeSubmit(), {
            wrapper: createWrapper(queryClient),
        });
        const createValues = createEmptyResumeForm('user-1');
        const updateValues = {
            ...createEmptyResumeForm('user-1'),
            resumeId: 8,
        };

        await expect(result.current.mutateAsync(createValues)).resolves.toEqual(
            {
                id: 5,
            },
        );
        await expect(result.current.mutateAsync(updateValues)).resolves.toEqual(
            {
                id: 8,
            },
        );

        expect(mocks.createResume).toHaveBeenCalledWith(createValues);
        expect(mocks.updateResume).toHaveBeenCalledWith(8, updateValues);
        expect(mocks.updateActiveResumeId).toHaveBeenCalledWith(5);
        expect(mocks.updateActiveResumeId).toHaveBeenCalledWith(8);
        expect(invalidateQueries).toHaveBeenCalledWith({
            queryKey: ['resumes'],
        });
        expect(invalidateQueries).toHaveBeenCalledWith({
            queryKey: ['resume', 8],
        });
    });

    it('deletes the active resume and invalidates list and detail queries', async () => {
        const queryClient = new QueryClient({
            defaultOptions: { mutations: { retry: false } },
        });
        const invalidateQueries = vi.spyOn(queryClient, 'invalidateQueries');
        mocks.deleteResumes.mockResolvedValue({ deletedIds: [2] });
        const { result } = renderHook(() => useResumeDelete(), {
            wrapper: createWrapper(queryClient),
        });

        await expect(result.current.mutateAsync(2)).resolves.toEqual({
            deletedIds: [2],
        });

        expect(mocks.deleteResumes).toHaveBeenCalledWith([2]);
        expect(mocks.updateActiveResumeId).toHaveBeenCalledWith(null);
        expect(invalidateQueries).toHaveBeenCalledWith({
            queryKey: ['resumes'],
        });
        expect(invalidateQueries).toHaveBeenCalledWith({
            queryKey: ['resume'],
        });
    });
});
