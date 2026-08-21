import { describe, expect, it, vi } from 'vitest';

import * as factories from '@/features/resume/model/factories';

describe('resume factories', () => {
    it('creates unique keys with crypto and fallback behavior', () => {
        const uuid = '00000000-0000-0000-0000-000000000000';
        const randomUUID = vi.spyOn(crypto, 'randomUUID').mockReturnValue(uuid);
        expect(factories.createClientKey()).toBe(uuid);
        randomUUID.mockRestore();
    });

    it('creates blank list-item defaults and a complete empty resume', () => {
        const itemFactories = [
            factories.createAddress,
            factories.createAward,
            factories.createCertification,
            factories.createEducation,
            factories.createHobby,
            factories.createLanguage,
            factories.createProject,
            factories.createSkill,
            factories.createSocialMediaLink,
            factories.createWorkExperience,
        ];
        for (const createItem of itemFactories)
            expect(createItem()).toMatchObject({ id: 0 });
        const form = factories.createEmptyResumeForm('user-1');
        expect(form.userId).toBe('user-1');
        expect(form.resumeTemplate).toBe('executive-clean');
        expect(form.certifications).toHaveLength(1);
        expect(form.education).toHaveLength(1);
        expect(form.projects).toHaveLength(1);
        expect(form.skills).toHaveLength(1);
        expect(form.workExperiences).toHaveLength(1);
    });
});
