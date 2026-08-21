import type { InputHTMLAttributes } from 'react';

import { cn } from '@/shared/lib/cn';

interface CheckboxFieldProps extends Omit<
    InputHTMLAttributes<HTMLInputElement>,
    'type'
> {
    description?: string;
    label: string;
}

export function CheckboxField({
    className,
    description,
    label,
    ...props
}: CheckboxFieldProps) {
    return (
        <label className="flex items-start gap-3 text-sm text-slate-800 dark:text-slate-100">
            <input
                {...props}
                className={cn(
                    'mt-0.5 h-5 w-5 rounded border-slate-300 text-teal-700 focus:ring-2 focus:ring-teal-700/50 dark:border-slate-600 dark:bg-slate-950',
                    className,
                )}
                type="checkbox"
            />
            <span>
                <span className="font-semibold">{label}</span>
                {description ? (
                    <span className="mt-1 block text-xs text-slate-600 dark:text-slate-300">
                        {description}
                    </span>
                ) : null}
            </span>
        </label>
    );
}
