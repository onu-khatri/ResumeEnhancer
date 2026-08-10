import { useMemo } from 'react';

import {
  createEmptyResumeForm,
  type ResumeFormValues,
} from '@/features/resume/model/types';

export interface ProgressItem {
  complete: boolean;
  label: string;
}

export function useResumeProgress(
  values: ResumeFormValues,
  userId: string | undefined,
) {
  const progressItems = useMemo<ProgressItem[]>(() => {
    const currentValues = values ?? createEmptyResumeForm(userId ?? '');

    return [
      { complete: currentValues.title.trim().length > 0, label: 'Title' },
      {
        complete:
          currentValues.personalInformation.email.trim().length > 0 ||
          currentValues.personalInformation.phoneNumber.trim().length > 0,
        label: 'Contact',
      },
      { complete: currentValues.summary.trim().length >= 80, label: 'Summary' },
      {
        complete: currentValues.skills.some((item) => item.skillName.trim()),
        label: 'Skills',
      },
      {
        complete: currentValues.education.some(
          (item) => item.institution.trim() || item.degree.trim(),
        ),
        label: 'Education',
      },
      {
        complete: currentValues.workExperiences.some(
          (item) => item.companyName.trim() || item.jobTitle.trim(),
        ),
        label: 'Experience',
      },
      {
        complete: currentValues.projects.some((item) => item.projectName.trim()),
        label: 'Projects',
      },
      {
        complete: currentValues.certifications.some((item) =>
          item.certificationName.trim(),
        ),
        label: 'Certifications',
      },
    ];
  }, [userId, values]);

  const completionPercent = Math.round(
    (progressItems.filter((item) => item.complete).length / progressItems.length) *
      100,
  );

  return { completionPercent, progressItems };
}
