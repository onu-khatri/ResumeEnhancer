import {
    EyeIcon,
    PencilSquareIcon,
    TrashIcon,
} from '@heroicons/react/24/outline';

import type { ResumeListItemResponse } from '@/features/resume/model/types';
import { formatDate } from '@/shared/lib/format';
import { Button } from '@/shared/ui/button';
import { Card } from '@/shared/ui/card';

export function ResumeDashboardList({
    items,
    onDelete,
    onEdit,
    onView,
}: {
    items: ResumeListItemResponse[];
    onDelete: (resume: ResumeListItemResponse) => void;
    onEdit: (resumeId: number) => void;
    onView: (resumeId: number) => void;
}) {
    return (
        <div className="grid gap-4 xl:grid-cols-2">
            {items.map((resume) => (
                <Card
                    key={resume.id}
                    className="rounded-[1.75rem] border-slate-300/70 bg-white/92 dark:border-slate-800 dark:bg-slate-950/90"
                >
                    <div className="flex flex-col gap-5">
                        <div className="flex items-start justify-between gap-4">
                            <div>
                                <p className="text-xs font-semibold tracking-[0.24em] text-teal-800 uppercase dark:text-teal-300">
                                    {resume.resumeTemplate ?? 'Resume'}
                                </p>
                                <h2 className="mt-2 text-2xl font-semibold text-slate-950 dark:text-white">
                                    {resume.title}
                                </h2>
                                <p className="mt-2 text-sm leading-7 text-slate-700 dark:text-slate-300">
                                    {resume.summary?.trim() ||
                                        'No summary added yet.'}
                                </p>
                            </div>
                            <div className="rounded-2xl border border-slate-200 bg-slate-50 px-3 py-2 text-sm font-medium text-slate-700 dark:border-slate-800 dark:bg-slate-900 dark:text-slate-300">
                                #{resume.id}
                            </div>
                        </div>

                        <div className="grid gap-3 sm:grid-cols-2">
                            <MetaTile
                                label="Updated"
                                value={formatDate(
                                    resume.app_UpdateDate ??
                                        resume.app_CreateDate,
                                )}
                            />
                            <MetaTile
                                label="Sections"
                                value={`${resume.educationCount} edu • ${resume.workExperienceCount} exp`}
                            />
                            <MetaTile
                                label="Skills"
                                value={`${resume.skillCount} skills • ${resume.projectCount} projects`}
                            />
                            <MetaTile
                                label="Certifications"
                                value={`${resume.certificationCount} certificates`}
                            />
                        </div>

                        <div className="flex flex-wrap gap-3">
                            <Button
                                onClick={() => onView(resume.id)}
                                variant="outline"
                            >
                                <EyeIcon className="h-4 w-4" />
                                View
                            </Button>
                            <Button
                                onClick={() => onEdit(resume.id)}
                                variant="outline"
                            >
                                <PencilSquareIcon className="h-4 w-4" />
                                Edit
                            </Button>
                            <Button
                                onClick={() => onDelete(resume)}
                                variant="ghost"
                            >
                                <TrashIcon className="h-4 w-4" />
                                Delete
                            </Button>
                        </div>
                    </div>
                </Card>
            ))}
        </div>
    );
}

function MetaTile({ label, value }: { label: string; value: string }) {
    return (
        <div className="rounded-2xl border border-slate-200 bg-slate-50 px-4 py-3 dark:border-slate-800 dark:bg-slate-900">
            <p className="text-xs tracking-[0.22em] text-slate-600 uppercase dark:text-slate-400">
                {label}
            </p>
            <p className="mt-1 text-sm font-medium text-slate-900 dark:text-white">
                {value}
            </p>
        </div>
    );
}
