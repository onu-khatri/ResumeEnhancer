import type { ResumeDetailResponse } from '@/features/resume/model/types';
import { PreviewSection } from '@/features/resume/preview/components/preview-section';
import { TimelineCard } from '@/features/resume/preview/components/timeline-card';
import { formatDate, formatDateRange } from '@/shared/lib/format';

export function PreviewMainColumn({ resume }: { resume: ResumeDetailResponse }) {
  return (
    <div className="space-y-8">
      <ExperienceSection resume={resume} />
      <ProjectsSection resume={resume} />
      <div className="grid gap-8 lg:grid-cols-2">
        <EducationSection resume={resume} />
        <CertificationsSection resume={resume} />
      </div>
    </div>
  );
}

function ExperienceSection({ resume }: { resume: ResumeDetailResponse }) {
  return (
    <PreviewSection title="Work experience">
      <div className="space-y-5">
        {resume.workExperiences.length ? (
          resume.workExperiences.map((experience) => (
            <TimelineCard
              key={experience.id}
              eyebrow={formatDateRange(experience.startDate, experience.endDate, experience.isCurrent)}
              subtitle={experience.companyName ?? 'Company name'}
              text={experience.description ?? 'No description added.'}
              title={experience.jobTitle ?? 'Role title'}
            />
          ))
        ) : (
          <p className="text-sm text-slate-500">No work experience has been added yet.</p>
        )}
      </div>
    </PreviewSection>
  );
}

function ProjectsSection({ resume }: { resume: ResumeDetailResponse }) {
  return (
    <PreviewSection title="Projects">
      <div className="space-y-5">
        {resume.projects.length ? (
          resume.projects.map((project) => (
            <TimelineCard
              key={project.id}
              eyebrow={formatDateRange(project.startDate, project.endDate, project.isCurrent)}
              subtitle={project.technologiesUsed ?? 'Technologies not listed'}
              text={project.description ?? 'No project details added.'}
              title={project.projectName}
            />
          ))
        ) : (
          <p className="text-sm text-slate-500">No projects have been added yet.</p>
        )}
      </div>
    </PreviewSection>
  );
}

function EducationSection({ resume }: { resume: ResumeDetailResponse }) {
  return (
    <PreviewSection title="Education">
      <div className="space-y-5">
        {resume.education.length ? (
          resume.education.map((education) => (
            <TimelineCard
              key={education.id}
              eyebrow={education.passingYear ? education.passingYear.toString() : 'Education'}
              subtitle={education.institution ?? 'Institution'}
              text={education.description ?? 'No notes added.'}
              title={education.degree ?? 'Degree'}
            />
          ))
        ) : (
          <p className="text-sm text-slate-500">No education records yet.</p>
        )}
      </div>
    </PreviewSection>
  );
}

function CertificationsSection({ resume }: { resume: ResumeDetailResponse }) {
  return (
    <PreviewSection title="Certifications">
      <div className="space-y-5">
        {resume.certifications.length ? (
          resume.certifications.map((certification) => (
            <TimelineCard
              key={certification.id}
              eyebrow={formatDate(certification.issueDate)}
              subtitle={certification.issuingOrganization ?? 'Issuing organization'}
              text={certification.description ?? 'No notes added.'}
              title={certification.certificationName}
            />
          ))
        ) : (
          <p className="text-sm text-slate-500">No certifications have been added yet.</p>
        )}
      </div>
    </PreviewSection>
  );
}
