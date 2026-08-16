import { createSocialMediaLink } from '@/features/resume/model/types';
import type {
    ResumeBuilderSectionProps,
    SocialLinkArray,
} from '@/features/resume/builder/builder-types';
import { RepeatingSection } from '@/features/resume/builder/components/repeating-section';
import { InputField } from '@/shared/ui/form-field';

export function SocialLinksSection({
    array,
    form,
}: ResumeBuilderSectionProps & { array: SocialLinkArray }) {
    return (
        <RepeatingSection
            errors={form.formState.errors.personalInformation?.socialMediaLinks}
            fields={array.fields}
            move={array.move}
            onAdd={() => array.append(createSocialMediaLink())}
            remove={array.remove}
            title="Social links"
        >
            {(_field, index) => (
                <div className="grid gap-4 md:grid-cols-3">
                    <InputField
                        error={
                            form.formState.errors.personalInformation
                                ?.socialMediaLinks?.[index]?.platform?.message
                        }
                        label="Platform"
                        placeholder="LinkedIn"
                        {...form.register(
                            `personalInformation.socialMediaLinks.${index}.platform`,
                        )}
                    />
                    <InputField
                        error={
                            form.formState.errors.personalInformation
                                ?.socialMediaLinks?.[index]?.displayName
                                ?.message
                        }
                        label="Display name"
                        placeholder="@yourhandle"
                        {...form.register(
                            `personalInformation.socialMediaLinks.${index}.displayName`,
                        )}
                    />
                    <InputField
                        className="md:col-span-3"
                        error={
                            form.formState.errors.personalInformation
                                ?.socialMediaLinks?.[index]?.url?.message
                        }
                        label="Profile URL"
                        placeholder="https://linkedin.com/in/your-profile"
                        {...form.register(
                            `personalInformation.socialMediaLinks.${index}.url`,
                        )}
                    />
                </div>
            )}
        </RepeatingSection>
    );
}
