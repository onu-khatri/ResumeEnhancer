import {
  resumeTemplateOptions,
  type ResumeFormValues,
} from '@/features/resume/model/types';
import type { ResumeBuilderSectionProps } from '@/features/resume/builder/builder-types';
import { BuilderCardSection } from '@/features/resume/builder/components/builder-card-section';
import { InputField, TextareaField } from '@/shared/ui/form-field';
import { SelectField } from '@/shared/ui/select-field';

export function IdentitySection({
  form,
  values,
}: ResumeBuilderSectionProps & { values: ResumeFormValues }) {
  return (
    <BuilderCardSection
      description="Start with the basics recruiters look for first."
      title="Resume identity"
    >
      <div className="grid gap-5 md:grid-cols-2">
        <InputField
          error={form.formState.errors.title?.message}
          label="Resume title"
          placeholder="Senior full-stack engineer"
          required
          {...form.register('title')}
        />
        <SelectField
          error={form.formState.errors.resumeTemplate?.message}
          label="Template"
          onChange={(value) => form.setValue('resumeTemplate', value)}
          options={resumeTemplateOptions}
          value={values.resumeTemplate}
        />
        <InputField
          className="md:col-span-2"
          error={form.formState.errors.photo?.message}
          label="Photo URL"
          placeholder="https://cdn.example.com/profile-photo.jpg"
          {...form.register('photo')}
        />
        <TextareaField
          className="md:col-span-2"
          description="A sharp, evidence-based summary performs better than a generic objective."
          error={form.formState.errors.summary?.message}
          label="Professional summary"
          placeholder="Summarize your impact, focus areas, and differentiators."
          {...form.register('summary')}
        />
      </div>
    </BuilderCardSection>
  );
}
