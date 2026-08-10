import { useState } from 'react';
import { PlusIcon } from '@heroicons/react/24/outline';

import { useResumeDashboard } from '@/features/resume/dashboard/use-resume-dashboard';
import { DashboardPagination } from '@/features/resume/dashboard/components/dashboard-pagination';
import { DashboardToolbar } from '@/features/resume/dashboard/components/dashboard-toolbar';
import { DeleteResumeDialog } from '@/features/resume/dashboard/components/delete-resume-dialog';
import { ResumeDashboardList } from '@/features/resume/dashboard/components/resume-dashboard-list';
import { ResumeShell } from '@/features/resume/layout/resume-shell';
import type { ResumeListItemResponse } from '@/features/resume/model/types';
import { Button } from '@/shared/ui/button';
import {
  EmptyState,
  ErrorState,
  InlineAlert,
  SkeletonBlock,
} from '@/shared/ui/status';

export function ResumeDashboardPage() {
  const dashboard = useResumeDashboard();
  const [resumeToDelete, setResumeToDelete] = useState<ResumeListItemResponse | null>(
    null,
  );

  const result = dashboard.resumeSearch.data;

  return (
    <ResumeShell
      actions={
        <Button onClick={dashboard.createResume}>
          <PlusIcon className="h-4 w-4" />
          Create new resume
        </Button>
      }
      description="Manage every saved resume in one place, then jump directly into preview or editing."
      eyebrow="Resume Dashboard"
      title="Resume dashboard"
    >
      <div className="space-y-6">
        <DashboardToolbar
          onCreateResume={dashboard.createResume}
          onSearchTextChange={dashboard.setSearchText}
          searchText={dashboard.searchText}
        />

        {dashboard.deleteResume.isError ? (
          <InlineAlert
            message={
              dashboard.deleteResume.error instanceof Error
                ? dashboard.deleteResume.error.message
                : 'The resume could not be deleted.'
            }
            title="Delete failed"
          />
        ) : null}

        {dashboard.resumeSearch.isPending ? (
          <DashboardSkeleton />
        ) : dashboard.resumeSearch.isError ? (
          <ErrorState
            description="We could not load the resume list from the API. Try again to refresh the dashboard."
            onRetry={() => void dashboard.resumeSearch.refetch()}
          />
        ) : result && result.items.length > 0 ? (
          <>
            <ResumeDashboardList
              items={result.items}
              onDelete={setResumeToDelete}
              onEdit={(resumeId) => dashboard.selectResume(resumeId, 'builder')}
              onView={(resumeId) => dashboard.selectResume(resumeId, 'preview')}
            />
            <DashboardPagination
              onPageChange={dashboard.setPageNumber}
              result={result}
            />
          </>
        ) : (
          <EmptyState
            action={<Button onClick={dashboard.createResume}>Create your first resume</Button>}
            description="No resumes match the current dashboard filters yet. Create one to get started."
            title="No resumes found"
          />
        )}
      </div>

      <DeleteResumeDialog
        isDeleting={dashboard.deleteResume.isPending}
        onClose={() => setResumeToDelete(null)}
        onConfirm={() => {
          if (resumeToDelete) {
            void dashboard.deleteResume.mutateAsync(resumeToDelete.id).then(() => {
              setResumeToDelete(null);
            });
          }
        }}
        resume={resumeToDelete}
      />
    </ResumeShell>
  );
}

function DashboardSkeleton() {
  return (
    <div className="grid gap-4 xl:grid-cols-2">
      {[1, 2, 3, 4].map((item) => (
        <div
          key={item}
          className="rounded-[1.75rem] border border-slate-200 bg-white/90 p-6 dark:border-slate-800 dark:bg-slate-950/90"
        >
          <SkeletonBlock className="h-4 w-28" />
          <SkeletonBlock className="mt-4 h-8 w-64" />
          <SkeletonBlock className="mt-3 h-16 w-full" />
          <div className="mt-5 grid gap-3 sm:grid-cols-2">
            <SkeletonBlock className="h-16" />
            <SkeletonBlock className="h-16" />
            <SkeletonBlock className="h-16" />
            <SkeletonBlock className="h-16" />
          </div>
          <div className="mt-5 flex gap-3">
            <SkeletonBlock className="h-11 w-24" />
            <SkeletonBlock className="h-11 w-24" />
            <SkeletonBlock className="h-11 w-24" />
          </div>
        </div>
      ))}
    </div>
  );
}
