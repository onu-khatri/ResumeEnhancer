import type {
  UseFieldArrayReturn,
  UseFormReturn,
} from 'react-hook-form';

import type {
  AwardFormValues,
  HobbyFormValues,
  LanguageFormValues,
  ResumeFormValues,
} from '@/features/resume/model/types';

export interface ResumeBuilderFieldArrays {
  awards: UseFieldArrayReturn<ResumeFormValues, 'personalInformation.awards'>;
  certifications: UseFieldArrayReturn<ResumeFormValues, 'certifications'>;
  education: UseFieldArrayReturn<ResumeFormValues, 'education'>;
  hobbies: UseFieldArrayReturn<ResumeFormValues, 'personalInformation.hobbies'>;
  languages: UseFieldArrayReturn<
    ResumeFormValues,
    'personalInformation.languages'
  >;
  projects: UseFieldArrayReturn<ResumeFormValues, 'projects'>;
  skills: UseFieldArrayReturn<ResumeFormValues, 'skills'>;
  socialLinks: UseFieldArrayReturn<
    ResumeFormValues,
    'personalInformation.socialMediaLinks'
  >;
  workExperiences: UseFieldArrayReturn<
    ResumeFormValues,
    'workExperiences'
  >;
}

export interface ResumeBuilderSectionProps {
  form: UseFormReturn<ResumeFormValues>;
}

export interface CompactArrayProps<TItem> {
  array: {
    append: (value: TItem) => void;
    fields: Array<{ id: string }>;
    remove: (index: number) => void;
  };
}

export type AwardCompactArrayProps = CompactArrayProps<AwardFormValues>;
export type HobbyCompactArrayProps = CompactArrayProps<HobbyFormValues>;
export type LanguageCompactArrayProps = CompactArrayProps<LanguageFormValues>;

export type CertificationArray = UseFieldArrayReturn<
  ResumeFormValues,
  'certifications'
>;
export type EducationArray = UseFieldArrayReturn<
  ResumeFormValues,
  'education'
>;
export type ProjectArray = UseFieldArrayReturn<ResumeFormValues, 'projects'>;
export type SkillArray = UseFieldArrayReturn<ResumeFormValues, 'skills'>;
export type SocialLinkArray = UseFieldArrayReturn<
  ResumeFormValues,
  'personalInformation.socialMediaLinks'
>;
export type WorkExperienceArray = UseFieldArrayReturn<
  ResumeFormValues,
  'workExperiences'
>;
