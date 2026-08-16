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
                'rounded-2xl border border-amber-300 bg-amber-50 px-4 py-3 text-sm text-amber-950 dark:border-amber-500/25 dark:bg-amber-500/10 dark:text-amber-100',
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
        <Card className="rounded-[2rem] border-dashed border-slate-300 bg-slate-50/80 p-10 text-center dark:border-slate-700 dark:bg-slate-900/60">
            <div className="mx-auto flex h-14 w-14 items-center justify-center rounded-2xl bg-teal-100 text-teal-800 dark:bg-teal-400/10 dark:text-teal-200">
                <ExclamationTriangleIcon className="h-6 w-6" />
            </div>
            <h2 className="mt-5 text-2xl font-semibold text-slate-950 dark:text-white">
                {title}
            </h2>
            <p className="mt-3 text-sm leading-6 text-slate-700 dark:text-slate-300">
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
        <Card className="rounded-[2rem] border-rose-300 bg-rose-50/90 p-8 dark:border-rose-500/25 dark:bg-rose-500/10">
            <p className="text-sm font-semibold text-rose-800 dark:text-rose-200">
                Something went wrong
            </p>
            <p className="mt-2 text-sm leading-6 text-rose-950 dark:text-rose-100">
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
