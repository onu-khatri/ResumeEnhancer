import { useLocation, useNavigate } from 'react-router-dom';

import { useAuth } from '@/features/auth/auth-context';
import { useResumeQuery } from '@/features/resume/hooks/use-resume-query';
import { ResumeShell } from '@/features/resume/layout/resume-shell';
import { PreviewHero } from '@/features/resume/preview/components/preview-hero';
import { PreviewMainColumn } from '@/features/resume/preview/components/preview-main-column';
import { PreviewSidebar } from '@/features/resume/preview/components/preview-sidebar';
import { PreviewSkeleton } from '@/features/resume/preview/components/preview-skeleton';
import { formatDate } from '@/shared/lib/format';
import { Button } from '@/shared/ui/button';
import { Card } from '@/shared/ui/card';
import { EmptyState, ErrorState, InlineAlert } from '@/shared/ui/status';

export function ResumePreviewPage() {
    const location = useLocation();
    const navigate = useNavigate();
    const { session } = useAuth();
    const resumeId = session?.resumeId ?? null;
    const resumeQuery = useResumeQuery(resumeId);
    const savedAt = (location.state as { savedAt?: string } | null)?.savedAt;

    if (resumeId === null) {
        return (
            <ResumeShell
                description="Your saved resume preview appears here after the first successful save."
                eyebrow="Resume Preview"
                title="Recruiter-ready preview"
            >
                <EmptyState
                    action={
                        <Button
                            onClick={() => navigate('/app/resume/dashboard')}
                        >
                            Open dashboard
                        </Button>
                    }
                    description="There is no active resume selected for this session. Pick one from the dashboard or create a new resume first."
                    title="No resume saved yet"
                />
            </ResumeShell>
        );
    }

    if (resumeQuery.isPending) {
        return <PreviewSkeleton />;
    }

    if (resumeQuery.isError) {
        return (
            <ResumeShell
                actions={
                    <Button
                        onClick={() => navigate('/app/resume/builder')}
                        variant="outline"
                    >
                        Edit builder
                    </Button>
                }
                description="Recover gracefully when the preview request fails."
                eyebrow="Resume Preview"
                title="Recruiter-ready preview"
            >
                <ErrorState
                    description="We could not fetch the saved resume details. Retry the request or return to the builder to keep editing locally."
                    onRetry={() => void resumeQuery.refetch()}
                />
            </ResumeShell>
        );
    }

    if (!resumeQuery.data) {
        return (
            <ResumeShell
                description="The preview is empty until the API returns saved resume details."
                eyebrow="Resume Preview"
                title="Recruiter-ready preview"
            >
                <EmptyState
                    action={
                        <Button onClick={() => navigate('/app/resume/builder')}>
                            Go to builder
                        </Button>
                    }
                    description="We did not receive a resume record from the API for the active session."
                    title="Nothing to preview"
                />
            </ResumeShell>
        );
    }

    const resume = resumeQuery.data;
    const contactDetails = [
        resume.personalInformation?.email,
        resume.personalInformation?.phoneNumber,
        resume.personalInformation?.address?.city,
    ].filter(Boolean) as string[];

    return (
        <ResumeShell
            actions={
                <Button
                    onClick={() => navigate('/app/resume/builder')}
                    variant="outline"
                >
                    Edit resume
                </Button>
            }
            description="A premium preview with strong typography, clear sectioning, and easy recovery paths if data is missing."
            eyebrow="Resume Preview"
            title="Recruiter-ready preview"
        >
            <div className="space-y-6">
                {savedAt ? (
                    <InlineAlert
                        message={`Your resume was saved ${formatDate(savedAt)} and the preview has been refreshed from the API.`}
                        title="Resume saved"
                    />
                ) : null}

                <Card className="overflow-hidden rounded-[2rem] border-white/60 bg-white/90 p-0 dark:border-white/10 dark:bg-slate-950/80">
                    <PreviewHero
                        contactDetails={contactDetails}
                        resume={resume}
                    />
                    <div className="grid gap-8 px-8 py-8 sm:px-10 lg:grid-cols-[0.8fr_1.2fr]">
                        <PreviewSidebar resume={resume} />
                        <PreviewMainColumn resume={resume} />
                    </div>
                </Card>
            </div>
        </ResumeShell>
    );
}
