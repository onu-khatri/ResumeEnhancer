import {
    Listbox,
    ListboxButton,
    ListboxOption,
    ListboxOptions,
} from '@headlessui/react';
import { CheckIcon, ChevronUpDownIcon } from '@heroicons/react/24/outline';

import { cn } from '@/shared/lib/cn';

export interface SelectOption {
    description?: string;
    label: string;
    value: string;
}

interface SelectFieldProps {
    error?: string;
    label: string;
    onChange: (value: string) => void;
    options: SelectOption[];
    value: string;
}

export function SelectField({
    error,
    label,
    onChange,
    options,
    value,
}: SelectFieldProps) {
    const selectedOption =
        options.find((option) => option.value === value) ?? options[0];

    return (
        <label className="grid gap-2">
            <span className="text-sm font-semibold text-slate-800 dark:text-slate-100">
                {label}
            </span>
            <Listbox value={selectedOption.value} onChange={onChange}>
                <div className="relative">
                    <ListboxButton
                        className={cn(
                            'flex h-12 w-full items-center justify-between rounded-2xl border border-slate-300 bg-white px-4 text-left text-sm text-slate-950 shadow-sm focus:border-teal-700 focus:ring-4 focus:ring-teal-700/12 focus:outline-none dark:border-slate-700 dark:bg-slate-950 dark:text-white',
                            error && 'border-rose-500 focus:ring-rose-600/12',
                        )}
                    >
                        <span className="block truncate">
                            {selectedOption.label}
                        </span>
                        <ChevronUpDownIcon className="h-5 w-5 text-slate-500" />
                    </ListboxButton>
                    <ListboxOptions className="absolute z-20 mt-2 max-h-60 w-full overflow-auto rounded-3xl border border-slate-300 bg-white p-2 shadow-2xl outline-none dark:border-slate-700 dark:bg-slate-900">
                        {options.map((option) => (
                            <ListboxOption
                                key={option.value}
                                className="group cursor-pointer rounded-2xl px-3 py-3 data-[focus]:bg-teal-50 dark:data-[focus]:bg-teal-400/10"
                                value={option.value}
                            >
                                <div className="flex items-start justify-between gap-3">
                                    <div>
                                        <p className="text-sm font-medium text-slate-900 dark:text-white">
                                            {option.label}
                                        </p>
                                        {option.description ? (
                                            <p className="mt-1 text-xs leading-5 text-slate-600 dark:text-slate-300">
                                                {option.description}
                                            </p>
                                        ) : null}
                                    </div>
                                    <CheckIcon className="invisible h-5 w-5 text-teal-700 group-data-[selected]:visible dark:text-teal-300" />
                                </div>
                            </ListboxOption>
                        ))}
                    </ListboxOptions>
                </div>
            </Listbox>
            {error ? (
                <span className="text-xs font-medium text-rose-700">
                    {error}
                </span>
            ) : null}
        </label>
    );
}
