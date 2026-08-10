import { createProject } from '@/features/resume/model/types';
import type {
  ProjectArray,
  ResumeBuilderSectionProps,
} from '@/features/resume/builder/builder-types';
import { RepeatingSection } from '@/features/resume/builder/components/repeating-section';
import { InputField, TextareaField } from '@/shared/ui/form-field';

export function ProjectsSection({
  array,
  form,
}: ResumeBuilderSectionProps & { array: ProjectArray }) {
  return (
    <RepeatingSection
      errors={form.formState.errors.projects}
      fields={array.fields}
      move={array.move}
      onAdd={() => array.append(createProject())}
      remove={array.remove}
      title="Projects"
    >
      {(_field, index) => (
        <div className="grid gap-4 md:grid-cols-2">
          <InputField
            error={form.formState.errors.projects?.[index]?.projectName?.message}
            label="Project name"
            placeholder="Resume intelligence workspace"
            {...form.register(`projects.${index}.projectName`)}
          />
          <InputField
            error={form.formState.errors.projects?.[index]?.role?.message}
            label="Role"
            placeholder="Lead frontend engineer"
            {...form.register(`projects.${index}.role`)}
          />
          <InputField
            error={form.formState.errors.projects?.[index]?.startDate?.message}
            label="Start date"
            type="date"
            {...form.register(`projects.${index}.startDate`)}
          />
          <InputField
            error={form.formState.errors.projects?.[index]?.endDate?.message}
            label="End date"
            type="date"
            {...form.register(`projects.${index}.endDate`)}
          />
          <InputField
            className="md:col-span-2"
            error={form.formState.errors.projects?.[index]?.technologiesUsed?.message}
            label="Technologies used"
            placeholder="React, TypeScript, Tailwind CSS, ASP.NET"
            {...form.register(`projects.${index}.technologiesUsed`)}
          />
          <TextareaField
            className="md:col-span-2"
            error={form.formState.errors.projects?.[index]?.description?.message}
            label="Project summary"
            placeholder="State the problem, your role, and the outcome."
            {...form.register(`projects.${index}.description`)}
          />
        </div>
      )}
    </RepeatingSection>
  );
}
