import { useQuery } from '@tanstack/react-query';

import { useAuth } from '@/features/auth/auth-context';
import { createResumeService } from '@/features/resume/api/resume-service';
import type { ResumeSearchRequest } from '@/features/resume/model/types';
import { createApiClient } from '@/shared/api/api-client';

export function useResumeSearch(request: ResumeSearchRequest) {
    const { logout, refreshSession, session } = useAuth();

    const apiClient = createApiClient({
        getAccessToken: () => session?.accessToken ?? null,
        onUnauthorized: logout,
        refreshSession,
    });

    const resumeService = createResumeService(apiClient);

    return useQuery({
        queryFn: () => resumeService.searchResumes(request),
        queryKey: ['resumes', request],
    });
}
