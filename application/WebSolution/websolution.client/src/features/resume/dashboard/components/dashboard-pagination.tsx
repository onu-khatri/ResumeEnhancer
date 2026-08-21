import type { ResumeSearchResponse } from '@/features/resume/model/types';
import { Pagination } from '@/shared/ui/pagination';

export function DashboardPagination({
    onPageChange,
    result,
}: {
    onPageChange: (pageNumber: number) => void;
    result: ResumeSearchResponse;
}) {
    return (
        <Pagination
            hasNextPage={result.hasNextPage}
            hasPreviousPage={result.hasPreviousPage}
            onPageChange={onPageChange}
            pageNumber={result.pageNumber}
            totalCount={result.totalCount}
            totalPages={result.totalPages}
        />
    );
}
