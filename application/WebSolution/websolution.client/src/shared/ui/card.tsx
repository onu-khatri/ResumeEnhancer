import type { HTMLAttributes } from 'react';

import { cn } from '@/shared/lib/cn';

export function Card({ className, ...props }: HTMLAttributes<HTMLDivElement>) {
    return (
        <div
            className={cn(
                'rounded-3xl border border-slate-300/70 bg-white p-6 shadow-[0_18px_40px_rgba(15,23,42,0.08)] ring-1 ring-white/70 dark:border-slate-700 dark:bg-slate-900 dark:ring-white/5',
                className,
            )}
            {...props}
        />
    );
}
