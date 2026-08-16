import { createCertification } from '@/features/resume/model/types';
import type {
    CertificationArray,
    ResumeBuilderSectionProps,
} from '@/features/resume/builder/builder-types';
import { RepeatingSection } from '@/features/resume/builder/components/repeating-section';
import { InputField, TextareaField } from '@/shared/ui/form-field';

export function CertificationsSection({
    array,
    form,
}: ResumeBuilderSectionProps & { array: CertificationArray }) {
    return (
        <RepeatingSection
            errors={form.formState.errors.certifications}
            fields={array.fields}
            move={array.move}
            onAdd={() => array.append(createCertification())}
            remove={array.remove}
            title="Certifications"
        >
            {(_field, index) => (
                <div className="grid gap-4 md:grid-cols-2">
                    <InputField
                        error={
                            form.formState.errors.certifications?.[index]
                                ?.certificationName?.message
                        }
                        label="Certification name"
                        placeholder="AWS Certified Developer"
                        {...form.register(
                            `certifications.${index}.certificationName`,
                        )}
                    />
                    <InputField
                        error={
                            form.formState.errors.certifications?.[index]
                                ?.issuingOrganization?.message
                        }
                        label="Issuer"
                        placeholder="Amazon Web Services"
                        {...form.register(
                            `certifications.${index}.issuingOrganization`,
                        )}
                    />
                    <InputField
                        error={
                            form.formState.errors.certifications?.[index]
                                ?.issueDate?.message
                        }
                        label="Issue date"
                        type="date"
                        {...form.register(`certifications.${index}.issueDate`)}
                    />
                    <InputField
                        error={
                            form.formState.errors.certifications?.[index]
                                ?.expirationDate?.message
                        }
                        label="Expiration date"
                        type="date"
                        {...form.register(
                            `certifications.${index}.expirationDate`,
                        )}
                    />
                    <InputField
                        error={
                            form.formState.errors.certifications?.[index]
                                ?.credentialId?.message
                        }
                        label="Credential ID"
                        {...form.register(
                            `certifications.${index}.credentialId`,
                        )}
                    />
                    <InputField
                        error={
                            form.formState.errors.certifications?.[index]
                                ?.credentialUrl?.message
                        }
                        label="Credential URL"
                        placeholder="https://www.credly.com/badges/..."
                        {...form.register(
                            `certifications.${index}.credentialUrl`,
                        )}
                    />
                    <TextareaField
                        className="md:col-span-2"
                        error={
                            form.formState.errors.certifications?.[index]
                                ?.description?.message
                        }
                        label="Details"
                        placeholder="What capability or depth does this validate?"
                        {...form.register(
                            `certifications.${index}.description`,
                        )}
                    />
                </div>
            )}
        </RepeatingSection>
    );
}
