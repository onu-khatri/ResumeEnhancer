import {
    ArrowPathIcon,
    ExclamationTriangleIcon,
} from '@heroicons/react/24/outline';
import type { ReactNode } from 'react';

import { cn } from '@/shared/lib/cn';
import { Button } from '@/shared/ui/button';
import { Card } from '@/shared/ui/card';

export function InlineAlert({
    className,
    message,
    title,
}: {
    className?: string;
    message: string;
    title: string;
}) {
    return (
        <div
            className={cn(
                'rounded-2xl border border-[var(--color-warning)] bg-[var(--color-warning-soft)] px-4 py-3 text-sm text-[var(--color-text)]',
                className,
            )}
        >
            <p className="font-semibold">{title}</p>
            <p className="mt-1">{message}</p>
        </div>
    );
}

export function EmptyState({
    action,
    description,
    title,
}: {
    action?: ReactNode;
    description: string;
    title: string;
}) {
    return (
        <Card className="rounded-[2rem] border-dashed bg-[var(--surface-subtle)] p-10 text-center">
            <div className="mx-auto flex h-14 w-14 items-center justify-center rounded-2xl bg-[var(--color-accent-soft)] text-[var(--color-accent-strong)]">
                <ExclamationTriangleIcon className="h-6 w-6" />
            </div>
            <h2 className="mt-5 text-2xl font-semibold text-[var(--color-heading)]">
                {title}
            </h2>
            <p className="mt-3 text-sm leading-6 text-[var(--color-text-muted)]">
                {description}
            </p>
            {action ? <div className="mt-6">{action}</div> : null}
        </Card>
    );
}

export function ErrorState({
    description,
    onRetry,
}: {
    description: string;
    onRetry?: () => void;
}) {
    return (
        <Card className="rounded-[2rem] border-[var(--color-danger)] bg-[var(--color-danger-soft)] p-8">
            <p className="text-sm font-semibold text-[var(--color-danger)]">
                Something went wrong
            </p>
            <p className="mt-2 text-sm leading-6 text-[var(--color-text)]">
                {description}
            </p>
            {onRetry ? (
                <Button className="mt-5" onClick={onRetry} variant="outline">
                    <ArrowPathIcon className="h-4 w-4" />
                    Try again
                </Button>
            ) : null}
        </Card>
    );
}

export function SkeletonBlock({ className }: { className?: string }) {
    return (
        <div
            className={cn(
                'animate-pulse rounded-2xl bg-slate-300/80 dark:bg-slate-800',
                className,
            )}
        />
    );
}
