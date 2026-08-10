import { Button } from '@/shared/ui/button';
import { ErrorState } from '@/shared/ui/status';
import { ResumeShell } from '@/features/resume/layout/resume-shell';
import { BuilderAlerts } from '@/features/resume/builder/components/builder-alerts';
import { BuilderSidebar } from '@/features/resume/builder/components/builder-sidebar';
import { BuilderSkeleton } from '@/features/resume/builder/components/builder-skeleton';
import { CertificationsSection } from '@/features/resume/builder/sections/certifications-section';
import { EducationSection } from '@/features/resume/builder/sections/education-section';
import { IdentitySection } from '@/features/resume/builder/sections/identity-section';
import { PersonalDetailsSection } from '@/features/resume/builder/sections/personal-details-section';
import { PersonalExtrasSection } from '@/features/resume/builder/sections/personal-extras-section';
import { ProjectsSection } from '@/features/resume/builder/sections/projects-section';
import { SkillsSection } from '@/features/resume/builder/sections/skills-section';
import { SocialLinksSection } from '@/features/resume/builder/sections/social-links-section';
import { WorkExperienceSection } from '@/features/resume/builder/sections/work-experience-section';
import { useResumeBuilderController } from '@/features/resume/builder/use-resume-builder-controller';
import { useResumeProgress } from '@/features/resume/builder/use-resume-progress';

export function ResumeBuilderPage() {
  const controller = useResumeBuilderController();
  const { sessionUserId } = {
    sessionUserId: controller.form.getValues('userId') || undefined,
  };
  const { completionPercent, progressItems } = useResumeProgress(
    controller.values,
    sessionUserId,
  );

  if (controller.resumeQuery.isPending && !controller.canHydrate) {
    return <BuilderSkeleton />;
  }

  if (controller.resumeQuery.isError && !controller.canHydrate) {
    return (
      <ResumeShell
        description="Recover from API errors without losing the editing surface."
        eyebrow="Resume Builder"
        title="Resume details"
      >
        <ErrorState
          description="We could not load the existing resume details. You can retry, or continue with a fresh draft once the backend responds again."
          onRetry={controller.retryResumeLoad}
        />
      </ResumeShell>
    );
  }

  return (
    <ResumeShell
      actions={
        <Button onClick={controller.saveResume}>
          {controller.submitResume.isPending ? 'Saving...' : 'Save and preview'}
        </Button>
      }
      description="Capture the full resume story with low-friction editing, resilient draft autosave, and API-backed create or update flows."
      eyebrow="Resume Builder"
      title="Resume details"
    >
      <div className="grid gap-6 xl:grid-cols-[minmax(0,1fr)_320px]">
        <div className="space-y-6">
          <BuilderAlerts
            draft={controller.matchingDraft}
            saveError={
              controller.submitResume.error instanceof Error
                ? controller.submitResume.error
                : null
            }
          />
          <IdentitySection form={controller.form} values={controller.values} />
          <PersonalDetailsSection form={controller.form} />
          <SocialLinksSection array={controller.arrays.socialLinks} form={controller.form} />
          <SkillsSection array={controller.arrays.skills} form={controller.form} />
          <EducationSection array={controller.arrays.education} form={controller.form} />
          <WorkExperienceSection
            array={controller.arrays.workExperiences}
            form={controller.form}
          />
          <ProjectsSection array={controller.arrays.projects} form={controller.form} />
          <CertificationsSection
            array={controller.arrays.certifications}
            form={controller.form}
          />
          <PersonalExtrasSection
            awards={controller.arrays.awards}
            form={controller.form}
            hobbies={controller.arrays.hobbies}
            languages={controller.arrays.languages}
          />
        </div>

        <BuilderSidebar
          clearDraft={controller.clearDraft}
          completionPercent={completionPercent}
          lastSavedAt={controller.lastSavedAt}
          progressItems={progressItems}
        />
      </div>
    </ResumeShell>
  );
}
