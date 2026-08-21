import { QueryClientProvider } from '@tanstack/react-query';
import type { PropsWithChildren } from 'react';

import { queryClient } from '@/app/query-client';
import { AuthProvider } from '@/features/auth/auth-context';
import { ThemeProvider } from '@/app/theme-provider';

export function AppProviders({ children }: PropsWithChildren) {
    return (
        <ThemeProvider>
            <QueryClientProvider client={queryClient}>
                <AuthProvider>{children}</AuthProvider>
            </QueryClientProvider>
        </ThemeProvider>
    );
}
