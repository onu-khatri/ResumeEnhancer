import type { SelectOption } from '@/shared/ui/select-field';

export const resumeTemplateOptions: SelectOption[] = [
  {
    description: 'Balanced, recruiter-friendly layout with strong readability.',
    label: 'Executive Clean',
    value: 'executive-clean',
  },
  {
    description: 'A denser layout for technical experience and project depth.',
    label: 'Modern Technical',
    value: 'modern-technical',
  },
  {
    description: 'A calmer, story-led layout for product and creative roles.',
    label: 'Narrative Portfolio',
    value: 'narrative-portfolio',
  },
];
