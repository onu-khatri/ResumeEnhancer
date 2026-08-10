import { createEducation } from '@/features/resume/model/types';
import type {
  EducationArray,
  ResumeBuilderSectionProps,
} from '@/features/resume/builder/builder-types';
import { RepeatingSection } from '@/features/resume/builder/components/repeating-section';
import { InputField, TextareaField } from '@/shared/ui/form-field';

export function EducationSection({
  array,
  form,
}: ResumeBuilderSectionProps & { array: EducationArray }) {
  return (
    <RepeatingSection
      errors={form.formState.errors.education}
      fields={array.fields}
      move={array.move}
      onAdd={() => array.append(createEducation())}
      remove={array.remove}
      title="Education"
    >
      {(_field, index) => (
        <div className="grid gap-4 md:grid-cols-2">
          <InputField
            error={form.formState.errors.education?.[index]?.degree?.message}
            label="Degree"
            placeholder="B.Tech in Computer Science"
            {...form.register(`education.${index}.degree`)}
          />
          <InputField
            error={form.formState.errors.education?.[index]?.institution?.message}
            label="Institution"
            placeholder="Indian Institute of Technology"
            {...form.register(`education.${index}.institution`)}
          />
          <InputField
            error={form.formState.errors.education?.[index]?.passingYear?.message}
            label="Passing year"
            placeholder="2023"
            {...form.register(`education.${index}.passingYear`)}
          />
          <InputField
            error={form.formState.errors.education?.[index]?.percentage?.message}
            label="Percentage"
            placeholder="84.5"
            {...form.register(`education.${index}.percentage`)}
          />
          <InputField
            error={form.formState.errors.education?.[index]?.grade?.message}
            label="Grade"
            placeholder="A"
            {...form.register(`education.${index}.grade`)}
          />
          <InputField
            error={form.formState.errors.education?.[index]?.city?.message}
            label="City"
            {...form.register(`education.${index}.city`)}
          />
          <TextareaField
            className="md:col-span-2"
            error={form.formState.errors.education?.[index]?.description?.message}
            label="Highlights"
            placeholder="Capstone, scholarship, leadership, or distinctions."
            {...form.register(`education.${index}.description`)}
          />
        </div>
      )}
    </RepeatingSection>
  );
}
