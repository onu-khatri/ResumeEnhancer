import { useId, useMemo } from 'react';

import { cn } from '@/shared/lib/cn';

export function Autocomplete({
    label,
    onChange,
    options,
    value,
}: {
    label: string;
    onChange: (value: string) => void;
    options: readonly string[];
    value: string;
}) {
    const listId = useId();
    const normalizedOptions = useMemo(
        () => Array.from(new Set(options)),
        [options],
    );
    return (
        <label className="grid gap-2">
            <span className="text-sm font-semibold text-slate-800 dark:text-slate-100">
                {label}
            </span>
            <input
                className={cn(
                    'h-12 rounded-lg border border-slate-300 bg-white px-4 text-sm shadow-sm focus:border-teal-700 focus:ring-2 focus:ring-teal-700/20 focus:outline-none dark:border-slate-700 dark:bg-slate-950',
                )}
                list={listId}
                onChange={(event) => onChange(event.target.value)}
                value={value}
            />
            <datalist id={listId}>
                {normalizedOptions.map((option) => (
                    <option key={option} value={option} />
                ))}
            </datalist>
        </label>
    );
}
