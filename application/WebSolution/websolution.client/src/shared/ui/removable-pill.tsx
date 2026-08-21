import { XMarkIcon } from '@heroicons/react/24/outline';

export function RemovablePill({
    label,
    onRemove,
}: {
    label: string;
    onRemove: () => void;
}) {
    return (
        <span className="inline-flex items-center gap-1 rounded-full border border-teal-200 bg-teal-50 px-3 py-1 text-sm text-teal-950 dark:border-teal-400/20 dark:bg-teal-400/10 dark:text-teal-100">
            {label}
            <button
                aria-label={`Remove ${label}`}
                className="rounded-full p-0.5 hover:bg-teal-200 dark:hover:bg-teal-400/20"
                onClick={onRemove}
                type="button"
            >
                <XMarkIcon className="h-4 w-4" />
            </button>
        </span>
    );
}
