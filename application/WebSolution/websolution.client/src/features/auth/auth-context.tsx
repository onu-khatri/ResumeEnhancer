/* eslint-disable react-refresh/only-export-components */
import type { PropsWithChildren } from 'react';

import { useAuthStore } from '@/features/auth/auth-store';

export function AuthProvider({ children }: PropsWithChildren) {
  return children;
}

export function useAuth() {
  const isReady = useAuthStore((state) => state.isReady);
  const logout = useAuthStore((state) => state.logout);
  const refreshSession = useAuthStore((state) => state.refreshSession);
  const session = useAuthStore((state) => state.session);
  const signIn = useAuthStore((state) => state.signIn);
  const updateActiveResumeId = useAuthStore(
    (state) => state.updateActiveResumeId,
  );

  return {
    isAuthenticated: session !== null,
    isReady,
    logout,
    refreshSession,
    session,
    signIn,
    updateActiveResumeId,
  };
}
