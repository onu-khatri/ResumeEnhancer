import { ResumeShell } from '@/features/resume/layout/resume-shell';
import { Card } from '@/shared/ui/card';
import { SkeletonBlock } from '@/shared/ui/status';

export function BuilderSkeleton() {
    return (
        <ResumeShell
            description="Loading your current resume details and local draft context."
            eyebrow="Resume Builder"
            title="Resume details"
        >
            <div className="grid gap-6 xl:grid-cols-[minmax(0,1fr)_320px]">
                <div className="space-y-6">
                    {[1, 2, 3].map((section) => (
                        <Card key={section} className="rounded-[2rem] p-6">
                            <SkeletonBlock className="h-8 w-48" />
                            <SkeletonBlock className="mt-3 h-4 w-80" />
                            <div className="mt-6 grid gap-4 md:grid-cols-2">
                                <SkeletonBlock className="h-12" />
                                <SkeletonBlock className="h-12" />
                                <SkeletonBlock className="h-28 md:col-span-2" />
                            </div>
                        </Card>
                    ))}
                </div>
                <Card className="rounded-[2rem] p-6">
                    <SkeletonBlock className="h-6 w-24" />
                    <SkeletonBlock className="mt-4 h-10 w-20" />
                    <SkeletonBlock className="mt-4 h-2 w-full" />
                </Card>
            </div>
        </ResumeShell>
    );
}
