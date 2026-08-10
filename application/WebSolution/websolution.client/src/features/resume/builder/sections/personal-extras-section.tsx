import {
  createAward,
  createHobby,
  createLanguage,
} from '@/features/resume/model/types';
import type {
  AwardCompactArrayProps,
  HobbyCompactArrayProps,
  LanguageCompactArrayProps,
  ResumeBuilderSectionProps,
} from '@/features/resume/builder/builder-types';
import { CompactSection } from '@/features/resume/builder/components/compact-section';
import { InputField } from '@/shared/ui/form-field';

export function PersonalExtrasSection({
  awards,
  form,
  hobbies,
  languages,
}: ResumeBuilderSectionProps & {
  awards: AwardCompactArrayProps['array'];
  hobbies: HobbyCompactArrayProps['array'];
  languages: LanguageCompactArrayProps['array'];
}) {
  return (
    <div className="grid gap-6 lg:grid-cols-3">
      <CompactSection array={awards} onAdd={createAward} title="Awards">
        {(index) => (
          <>
            <InputField
              error={form.formState.errors.personalInformation?.awards?.[index]?.awardName?.message}
              label="Award"
              {...form.register(`personalInformation.awards.${index}.awardName`)}
            />
            <InputField
              error={form.formState.errors.personalInformation?.awards?.[index]?.issuingOrganization?.message}
              label="Organization"
              {...form.register(
                `personalInformation.awards.${index}.issuingOrganization`,
              )}
            />
          </>
        )}
      </CompactSection>

      <CompactSection array={languages} onAdd={createLanguage} title="Languages">
        {(index) => (
          <>
            <InputField
              error={form.formState.errors.personalInformation?.languages?.[index]?.languageName?.message}
              label="Language"
              {...form.register(`personalInformation.languages.${index}.languageName`)}
            />
            <InputField
              error={form.formState.errors.personalInformation?.languages?.[index]?.proficiencyLevel?.message}
              label="Level"
              placeholder="Professional working proficiency"
              {...form.register(
                `personalInformation.languages.${index}.proficiencyLevel`,
              )}
            />
          </>
        )}
      </CompactSection>

      <CompactSection array={hobbies} onAdd={createHobby} title="Hobbies">
        {(index) => (
          <InputField
            error={form.formState.errors.personalInformation?.hobbies?.[index]?.hobbyName?.message}
            label="Hobby"
            {...form.register(`personalInformation.hobbies.${index}.hobbyName`)}
          />
        )}
      </CompactSection>
    </div>
  );
}
