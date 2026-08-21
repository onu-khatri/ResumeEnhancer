import { describe, expect, it } from 'vitest';

import { createEmptyResumeForm } from '@/features/resume/model/factories';
import { resumeFormSchema } from '@/features/resume/model/schema';

describe('resume form schema', () => {
    it('accepts a valid normalized resume form', () => {
        const form = createEmptyResumeForm('user-1');
        form.title = ' Resume ';
        form.certifications[0].certificationName = 'Certification';
        form.projects[0].projectName = 'Project';
        form.skills[0].skillName = 'Skill';
        form.personalInformation.socialMediaLinks[0].platform = 'LinkedIn';
        form.personalInformation.socialMediaLinks[0].url =
            'https://example.com';
        expect(resumeFormSchema.parse(form).title).toBe('Resume');
    });

    it('rejects invalid required, URL, date, and numeric fields', () => {
        const form = createEmptyResumeForm('');
        form.photo = 'not-a-url';
        form.certifications[0].issueDate = 'invalid-date';
        form.education[0].percentage = '101';
        expect(resumeFormSchema.safeParse(form).success).toBe(false);
    });
});
