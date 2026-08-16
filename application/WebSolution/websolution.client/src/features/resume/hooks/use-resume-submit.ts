import { useMutation, useQueryClient } from '@tanstack/react-query';

import { useAuth } from '@/features/auth/auth-context';
import { createResumeService } from '@/features/resume/api/resume-service';
import type { ResumeFormValues } from '@/features/resume/model/types';
import { createApiClient } from '@/shared/api/api-client';

export function useResumeSubmit() {
    const queryClient = useQueryClient();
    const { logout, refreshSession, session, updateActiveResumeId } = useAuth();

    const apiClient = createApiClient({
        getAccessToken: () => session?.accessToken ?? null,
        onUnauthorized: logout,
        refreshSession,
    });

    const resumeService = createResumeService(apiClient);

    return useMutation({
        mutationFn: async (values: ResumeFormValues) => {
            const response =
                values.resumeId === null
                    ? await resumeService.createResume(values)
                    : await resumeService.updateResume(values.resumeId, values);

            updateActiveResumeId(response.id);
            return response;
        },
        onSuccess: async (response) => {
            await queryClient.invalidateQueries({ queryKey: ['resumes'] });
            await queryClient.invalidateQueries({
                queryKey: ['resume', response.id],
            });
        },
    });
}
