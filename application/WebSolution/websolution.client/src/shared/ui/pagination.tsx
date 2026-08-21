import { Button } from '@/shared/ui/button';

export function Pagination({
    hasNextPage,
    hasPreviousPage,
    onPageChange,
    pageNumber,
    totalCount,
    totalPages,
}: {
    hasNextPage: boolean;
    hasPreviousPage: boolean;
    onPageChange: (page: number) => void;
    pageNumber: number;
    totalCount: number;
    totalPages: number;
}) {
    return (
        <nav
            aria-label="Pagination"
            className="flex flex-col gap-4 rounded-lg border border-slate-200 bg-white/70 px-5 py-4 sm:flex-row sm:items-center sm:justify-between dark:border-slate-800 dark:bg-slate-950/70"
        >
            <p className="text-sm text-slate-600 dark:text-slate-300">
                Showing page {pageNumber} of {totalPages || 1} with {totalCount}{' '}
                total results.
            </p>
            <div className="flex gap-3">
                <Button
                    disabled={!hasPreviousPage}
                    onClick={() => onPageChange(pageNumber - 1)}
                    variant="outline"
                >
                    Previous
                </Button>
                <Button
                    disabled={!hasNextPage}
                    onClick={() => onPageChange(pageNumber + 1)}
                    variant="outline"
                >
                    Next
                </Button>
            </div>
        </nav>
    );
}
