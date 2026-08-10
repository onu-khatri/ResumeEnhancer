import { useQuery } from '@tanstack/react-query';

import { useAuth } from '@/features/auth/auth-context';
import { createResumeService } from '@/features/resume/api/resume-service';
import { createApiClient } from '@/shared/api/api-client';

export function useResumeQuery(resumeId: number | null) {
  const { logout, refreshSession, session } = useAuth();

  const apiClient = createApiClient({
    getAccessToken: () => session?.accessToken ?? null,
    onUnauthorized: logout,
    refreshSession,
  });

  const resumeService = createResumeService(apiClient);

  return useQuery({
    enabled: resumeId !== null,
    queryFn: () => resumeService.getResume(resumeId!),
    queryKey: ['resume', resumeId],
  });
}
