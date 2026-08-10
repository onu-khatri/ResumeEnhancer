import { SparklesIcon } from '@heroicons/react/24/outline';

import type { ResumeDetailResponse } from '@/features/resume/model/types';
import { formatDate } from '@/shared/lib/format';

export function PreviewHero({
  contactDetails,
  resume,
}: {
  contactDetails: string[];
  resume: ResumeDetailResponse;
}) {
  return (
    <div className="bg-[linear-gradient(135deg,_rgba(15,118,110,0.12),_rgba(15,23,42,0.03),_rgba(180,83,9,0.06))] px-8 py-8 sm:px-10">
      <div className="flex flex-col gap-8 lg:flex-row lg:items-start lg:justify-between">
        <div>
          <p className="text-sm font-semibold uppercase tracking-[0.32em] text-teal-800 dark:text-teal-300">
            {resume.resumeTemplate ?? 'Executive Clean'}
          </p>
          <h1 className="mt-4 font-serif text-4xl font-semibold tracking-tight text-slate-950 dark:text-white sm:text-5xl">
            {resume.title}
          </h1>
          {contactDetails.length ? <ContactChips items={contactDetails} /> : null}
        </div>
        <LastUpdatedCard resume={resume} />
      </div>

      {resume.summary ? (
        <p className="mt-8 max-w-4xl text-base leading-8 text-slate-700 dark:text-slate-200">
          {resume.summary}
        </p>
      ) : null}
    </div>
  );
}

function ContactChips({ items }: { items: string[] }) {
  return (
    <div className="mt-4 flex flex-wrap gap-3">
      {items.map((detail) => (
        <span
          key={detail}
          className="rounded-full border border-slate-300 bg-white/88 px-4 py-2 text-sm text-slate-800 dark:border-slate-700 dark:bg-slate-900/80 dark:text-slate-200"
        >
          {detail}
        </span>
      ))}
    </div>
  );
}

function LastUpdatedCard({ resume }: { resume: ResumeDetailResponse }) {
  return (
    <div className="max-w-sm rounded-[1.75rem] border border-slate-300/70 bg-white/88 p-5 shadow-lg backdrop-blur-sm dark:border-slate-700 dark:bg-slate-900/75">
      <div className="flex items-center gap-3">
        <div className="rounded-2xl bg-teal-100 p-2 text-teal-800 dark:bg-teal-400/10 dark:text-teal-300">
          <SparklesIcon className="h-5 w-5" />
        </div>
        <div>
          <p className="text-xs uppercase tracking-[0.24em] text-slate-600 dark:text-slate-400">
            Last updated
          </p>
          <p className="text-sm font-medium text-slate-900 dark:text-white">
            {formatDate(resume.app_UpdateDate ?? resume.app_CreateDate, {
              day: 'numeric',
              month: 'long',
              year: 'numeric',
            })}
          </p>
        </div>
      </div>
      <p className="mt-4 text-sm leading-6 text-slate-600 dark:text-slate-300">
        This layout is tuned for fast scanning: clear hierarchy, strong spacing,
        and compact evidence blocks instead of dense walls of text.
      </p>
    </div>
  );
}
