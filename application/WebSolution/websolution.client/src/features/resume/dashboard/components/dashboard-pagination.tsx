import type { ResumeSearchResponse } from '@/features/resume/model/types';
import { Button } from '@/shared/ui/button';

export function DashboardPagination({
  onPageChange,
  result,
}: {
  onPageChange: (pageNumber: number) => void;
  result: ResumeSearchResponse;
}) {
  return (
    <div className="flex flex-col gap-4 rounded-[1.5rem] border border-slate-200 bg-white/70 px-5 py-4 dark:border-slate-800 dark:bg-slate-950/70 sm:flex-row sm:items-center sm:justify-between">
      <p className="text-sm text-slate-600 dark:text-slate-300">
        Showing page {result.pageNumber} of {result.totalPages || 1} with{' '}
        {result.totalCount} total resumes.
      </p>
      <div className="flex gap-3">
        <Button
          onClick={() => onPageChange(result.pageNumber - 1)}
          variant="outline"
          disabled={!result.hasPreviousPage}
        >
          Previous
        </Button>
        <Button
          onClick={() => onPageChange(result.pageNumber + 1)}
          variant="outline"
          disabled={!result.hasNextPage}
        >
          Next
        </Button>
      </div>
    </div>
  );
}
