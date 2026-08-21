import { cn } from '@/shared/lib/cn';

export interface RadioOption {
    description?: string;
    label: string;
    value: string;
}

export function RadioGroup({
    label,
    onChange,
    options,
    value,
}: {
    label: string;
    onChange: (value: string) => void;
    options: RadioOption[];
    value: string;
}) {
    return (
        <fieldset className="grid gap-2">
            <legend className="text-sm font-semibold text-slate-800 dark:text-slate-100">
                {label}
            </legend>
            <div className="grid gap-2 sm:grid-cols-2">
                {options.map((option) => (
                    <label
                        key={option.value}
                        className={cn(
                            'flex cursor-pointer gap-3 rounded-lg border p-3 text-sm transition focus-within:ring-2 focus-within:ring-teal-700/50',
                            value === option.value
                                ? 'border-teal-700 bg-teal-50 dark:bg-teal-400/10'
                                : 'border-slate-300 dark:border-slate-700',
                        )}
                    >
                        <input
                            aria-label={option.label}
                            checked={value === option.value}
                            name={label}
                            onChange={() => onChange(option.value)}
                            type="radio"
                            value={option.value}
                        />
                        <span>
                            <span className="font-semibold">
                                {option.label}
                            </span>
                            {option.description ? (
                                <span className="mt-1 block text-xs text-slate-600 dark:text-slate-300">
                                    {option.description}
                                </span>
                            ) : null}
                        </span>
                    </label>
                ))}
            </div>
        </fieldset>
    );
}
