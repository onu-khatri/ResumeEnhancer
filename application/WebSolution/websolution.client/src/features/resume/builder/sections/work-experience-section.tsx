import { createWorkExperience } from '@/features/resume/model/types';
import type {
  ResumeBuilderSectionProps,
  WorkExperienceArray,
} from '@/features/resume/builder/builder-types';
import { RepeatingSection } from '@/features/resume/builder/components/repeating-section';
import { InputField, TextareaField } from '@/shared/ui/form-field';

export function WorkExperienceSection({
  array,
  form,
}: ResumeBuilderSectionProps & { array: WorkExperienceArray }) {
  return (
    <RepeatingSection
      errors={form.formState.errors.workExperiences}
      fields={array.fields}
      move={array.move}
      onAdd={() => array.append(createWorkExperience())}
      remove={array.remove}
      title="Work experience"
    >
      {(_field, index) => (
        <div className="grid gap-4 md:grid-cols-2">
          <InputField
            error={form.formState.errors.workExperiences?.[index]?.jobTitle?.message}
            label="Job title"
            placeholder="Senior software engineer"
            {...form.register(`workExperiences.${index}.jobTitle`)}
          />
          <InputField
            error={form.formState.errors.workExperiences?.[index]?.companyName?.message}
            label="Company"
            placeholder="Acme Labs"
            {...form.register(`workExperiences.${index}.companyName`)}
          />
          <InputField
            error={form.formState.errors.workExperiences?.[index]?.startDate?.message}
            label="Start date"
            type="date"
            {...form.register(`workExperiences.${index}.startDate`)}
          />
          <InputField
            error={form.formState.errors.workExperiences?.[index]?.endDate?.message}
            label="End date"
            type="date"
            {...form.register(`workExperiences.${index}.endDate`)}
          />
          <InputField
            error={form.formState.errors.workExperiences?.[index]?.location?.message}
            label="Location"
            placeholder="Remote / Bengaluru"
            {...form.register(`workExperiences.${index}.location`)}
          />
          <label className="flex items-center gap-3 rounded-2xl border border-slate-200 px-4 py-3 text-sm font-medium text-slate-700 dark:border-slate-800 dark:text-slate-200">
            <input
              className="h-4 w-4 rounded border-slate-300 text-lime-600"
              type="checkbox"
              {...form.register(`workExperiences.${index}.isCurrent`)}
            />
            Current role
          </label>
          <TextareaField
            className="md:col-span-2"
            error={form.formState.errors.workExperiences?.[index]?.description?.message}
            label="Impact summary"
            placeholder="Describe the problems solved, scale, and results."
            {...form.register(`workExperiences.${index}.description`)}
          />
        </div>
      )}
    </RepeatingSection>
  );
}
