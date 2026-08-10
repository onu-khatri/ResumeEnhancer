import type { ResumeBuilderSectionProps } from '@/features/resume/builder/builder-types';
import { BuilderCardSection } from '@/features/resume/builder/components/builder-card-section';
import { InputField } from '@/shared/ui/form-field';

export function PersonalDetailsSection({ form }: ResumeBuilderSectionProps) {
  return (
    <BuilderCardSection
      description="Contact details, location, and public professional links."
      title="Personal details"
    >
      <div className="grid gap-5 md:grid-cols-2">
        <InputField
          error={form.formState.errors.personalInformation?.email?.message}
          label="Email"
          placeholder="name@example.com"
          {...form.register('personalInformation.email')}
        />
        <InputField
          error={form.formState.errors.personalInformation?.phoneNumber?.message}
          label="Phone number"
          placeholder="+1 555 234 9876"
          {...form.register('personalInformation.phoneNumber')}
        />
        <InputField
          error={form.formState.errors.personalInformation?.address?.line1?.message}
          label="Address line 1"
          placeholder="221B Baker Street"
          {...form.register('personalInformation.address.line1')}
        />
        <InputField
          error={form.formState.errors.personalInformation?.address?.line2?.message}
          label="Address line 2"
          placeholder="Apartment or suite"
          {...form.register('personalInformation.address.line2')}
        />
        <InputField
          error={form.formState.errors.personalInformation?.address?.city?.message}
          label="City"
          {...form.register('personalInformation.address.city')}
        />
        <InputField
          error={form.formState.errors.personalInformation?.address?.state?.message}
          label="State"
          {...form.register('personalInformation.address.state')}
        />
        <InputField
          error={form.formState.errors.personalInformation?.address?.country?.message}
          label="Country"
          {...form.register('personalInformation.address.country')}
        />
        <InputField
          error={form.formState.errors.personalInformation?.address?.postalCode?.message}
          label="Postal code"
          {...form.register('personalInformation.address.postalCode')}
        />
      </div>
    </BuilderCardSection>
  );
}
