import { CheckCircleIcon } from '@heroicons/react/24/outline';

import type { ProgressItem } from '@/features/resume/builder/use-resume-progress';
import { formatRelativeTime } from '@/shared/lib/format';
import { Button } from '@/shared/ui/button';
import { Card } from '@/shared/ui/card';

export function BuilderSidebar({
  clearDraft,
  completionPercent,
  lastSavedAt,
  progressItems,
}: {
  clearDraft: () => void;
  completionPercent: number;
  lastSavedAt: string | null;
  progressItems: ProgressItem[];
}) {
  return (
    <div className="space-y-6 xl:sticky xl:top-4 xl:self-start">
      <CompletionCard
        completionPercent={completionPercent}
        progressItems={progressItems}
      />
      <DraftStatusCard clearDraft={clearDraft} lastSavedAt={lastSavedAt} />
    </div>
  );
}

function CompletionCard({
  completionPercent,
  progressItems,
}: {
  completionPercent: number;
  progressItems: ProgressItem[];
}) {
  return (
    <Card className="rounded-[2rem]">
      <p className="text-sm font-semibold uppercase tracking-[0.24em] text-slate-600 dark:text-slate-300">
        Completion
      </p>
      <div className="mt-4 flex items-end gap-3">
        <span className="text-4xl font-semibold text-slate-950 dark:text-white">
          {completionPercent}%
        </span>
        <span className="pb-1 text-sm text-slate-600 dark:text-slate-400">
          ready to review
        </span>
      </div>
      <div className="mt-4 h-2 overflow-hidden rounded-full bg-slate-200 dark:bg-slate-800">
        <div
          className="h-full rounded-full bg-teal-700 transition-all duration-500 dark:bg-teal-400"
          style={{ width: `${completionPercent}%` }}
        />
      </div>
      <div className="mt-5 space-y-3">
        {progressItems.map((item) => (
          <div
            key={item.label}
            className="flex items-center justify-between rounded-2xl border border-slate-200 bg-slate-50 px-4 py-3 dark:border-slate-800 dark:bg-slate-900"
          >
            <span className="text-sm text-slate-700 dark:text-slate-200">
              {item.label}
            </span>
            <StatusBadge complete={item.complete} />
          </div>
        ))}
      </div>
    </Card>
  );
}

function DraftStatusCard({
  clearDraft,
  lastSavedAt,
}: {
  clearDraft: () => void;
  lastSavedAt: string | null;
}) {
  return (
    <Card className="rounded-[2rem]">
      <p className="text-sm font-semibold uppercase tracking-[0.24em] text-slate-600 dark:text-slate-300">
        Draft status
      </p>
      <p className="mt-3 text-sm leading-6 text-slate-700 dark:text-slate-300">
        Drafts autosave locally while you work, so accidental refreshes do not
        wipe progress.
      </p>
      <p className="mt-4 text-sm font-medium text-slate-900 dark:text-white">
        {lastSavedAt
          ? `Last autosave ${formatRelativeTime(lastSavedAt)}`
          : 'Autosave will begin once the form finishes hydrating.'}
      </p>
      <Button className="mt-5 w-full" onClick={clearDraft} variant="outline">
        Clear local draft
      </Button>
    </Card>
  );
}

function StatusBadge({ complete }: { complete: boolean }) {
  return (
    <span
      className={`inline-flex items-center gap-1 rounded-full px-3 py-1 text-xs font-semibold ${
        complete
          ? 'bg-teal-100 text-teal-900 dark:bg-teal-400/10 dark:text-teal-200'
          : 'bg-slate-200 text-slate-700 dark:bg-slate-800 dark:text-slate-300'
      }`}
    >
      {complete ? <CheckCircleIcon className="h-3.5 w-3.5" /> : null}
      {complete ? 'Done' : 'Pending'}
    </span>
  );
}
