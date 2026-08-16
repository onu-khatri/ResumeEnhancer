import { ResumeShell } from '@/features/resume/layout/resume-shell';
import { Card } from '@/shared/ui/card';
import { SkeletonBlock } from '@/shared/ui/status';

export function PreviewSkeleton() {
    return (
        <ResumeShell
            description="Loading the saved resume preview from the monolithic API."
            eyebrow="Resume Preview"
            title="Recruiter-ready preview"
        >
            <Card className="rounded-[2rem] p-8">
                <SkeletonBlock className="h-4 w-32" />
                <SkeletonBlock className="mt-4 h-14 w-80" />
                <SkeletonBlock className="mt-6 h-24 w-full" />
                <div className="mt-8 grid gap-6 lg:grid-cols-2">
                    <SkeletonBlock className="h-72" />
                    <SkeletonBlock className="h-72" />
                </div>
            </Card>
        </ResumeShell>
    );
}
