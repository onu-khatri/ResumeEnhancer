import type { ReactNode } from 'react';

import type { SharedRequestState } from '@/shared/lib/errors';
import { EmptyState, ErrorState, SkeletonBlock } from '@/shared/ui/status';

export function RequestState({
    children,
    errorMessage,
    onRetry,
    state,
}: {
    children: ReactNode;
    errorMessage?: string;
    onRetry?: () => void;
    state: SharedRequestState;
}) {
    if (state === 'loading') return <SkeletonBlock className="h-48 w-full" />;
    if (state === 'empty')
        return (
            <EmptyState
                description="There is no content to display yet."
                title="Nothing here yet"
            />
        );
    if (state === 'authorization')
        return (
            <EmptyState
                description="You do not have access to this content."
                title="Access restricted"
            />
        );
    if (state === 'entitlement')
        return (
            <EmptyState
                description="Your current plan does not include this feature."
                title="Upgrade required"
            />
        );
    if (state === 'offline')
        return (
            <ErrorState
                description="You appear to be offline. Reconnect and try again."
                onRetry={onRetry}
            />
        );
    if (state === 'error')
        return (
            <ErrorState
                description={
                    errorMessage ?? 'The request could not be completed.'
                }
                onRetry={onRetry}
            />
        );
    return children;
}
