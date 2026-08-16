import { createSkill } from '@/features/resume/model/types';
import type {
    ResumeBuilderSectionProps,
    SkillArray,
} from '@/features/resume/builder/builder-types';
import { RepeatingSection } from '@/features/resume/builder/components/repeating-section';
import { InputField, TextareaField } from '@/shared/ui/form-field';

export function SkillsSection({
    array,
    form,
}: ResumeBuilderSectionProps & { array: SkillArray }) {
    return (
        <RepeatingSection
            errors={form.formState.errors.skills}
            fields={array.fields}
            move={array.move}
            onAdd={() => array.append(createSkill())}
            remove={array.remove}
            title="Skills"
        >
            {(_field, index) => (
                <div className="grid gap-4 md:grid-cols-3">
                    <InputField
                        error={
                            form.formState.errors.skills?.[index]?.skillName
                                ?.message
                        }
                        label="Skill"
                        placeholder="React"
                        {...form.register(`skills.${index}.skillName`)}
                    />
                    <InputField
                        error={
                            form.formState.errors.skills?.[index]
                                ?.proficiencyLevel?.message
                        }
                        label="Proficiency"
                        placeholder="Advanced"
                        {...form.register(`skills.${index}.proficiencyLevel`)}
                    />
                    <InputField
                        error={
                            form.formState.errors.skills?.[index]
                                ?.yearsOfExperience?.message
                        }
                        label="Years of experience"
                        placeholder="4.5"
                        {...form.register(`skills.${index}.yearsOfExperience`)}
                    />
                    <TextareaField
                        className="md:col-span-3"
                        error={
                            form.formState.errors.skills?.[index]?.description
                                ?.message
                        }
                        label="Context"
                        placeholder="Platform migrations, design systems, or delivery scale."
                        {...form.register(`skills.${index}.description`)}
                    />
                </div>
            )}
        </RepeatingSection>
    );
}
