import { useMutation, useQueryClient } from '@tanstack/react-query';

import { useAuth } from '@/features/auth/auth-context';
import { createResumeService } from '@/features/resume/api/resume-service';
import { createApiClient } from '@/shared/api/api-client';

export function useResumeDelete() {
    const queryClient = useQueryClient();
    const { logout, refreshSession, session, updateActiveResumeId } = useAuth();

    const apiClient = createApiClient({
        getAccessToken: () => session?.accessToken ?? null,
        onUnauthorized: logout,
        refreshSession,
    });

    const resumeService = createResumeService(apiClient);

    return useMutation({
        mutationFn: async (resumeId: number) => {
            const response = await resumeService.deleteResumes([resumeId]);
            if (session?.resumeId === resumeId) {
                updateActiveResumeId(null);
            }
            return response;
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['resumes'] });
            await queryClient.invalidateQueries({ queryKey: ['resume'] });
        },
    });
}
