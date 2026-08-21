import { describe, expect, it, vi } from 'vitest';

import {
    createResumeService,
    mapResumeResponseToForm,
} from '@/features/resume/api/resume-service';
import {
    createEmptyResumeForm,
    type ResumeDetailResponse,
} from '@/features/resume/model/types';

describe('resume service', () => {
    it('maps nullable API response values into editable form defaults', () => {
        const form = mapResumeResponseToForm({
            app_CreateDate: '2026-01-01',
            app_UpdateDate: null,
            app_Version: [1],
            certifications: [],
            education: [],
            id: 4,
            personalInformation: null,
            photo: null,
            projects: [],
            resumeTemplate: null,
            skills: [],
            summary: null,
            title: 'Resume',
            userId: 'user-1',
            workExperiences: [],
        } satisfies ResumeDetailResponse);

        expect(form).toMatchObject({
            photo: '',
            resumeId: 4,
            resumeTemplate: 'executive-clean',
            summary: '',
            title: 'Resume',
            userId: 'user-1',
        });
        expect(form.personalInformation.address).toMatchObject({ id: 0 });
    });

    it('maps populated API entries and normalizes their date fields', () => {
        const form = mapResumeResponseToForm({
            app_CreateDate: '2026-01-01',
            app_UpdateDate: null,
            app_Version: [1],
            certifications: [
                {
                    certificationName: 'Cloud',
                    credentialId: null,
                    credentialUrl: null,
                    description: null,
                    expirationDate: null,
                    id: 1,
                    issueDate: '2025-02-03T00:00:00Z',
                    issuingOrganization: null,
                },
            ],
            education: [
                {
                    city: null,
                    degree: 'BSc',
                    description: null,
                    grade: null,
                    id: 2,
                    institution: 'University',
                    isCurrent: false,
                    passingYear: 2020,
                    percentage: 80,
                    state: null,
                },
            ],
            id: 4,
            personalInformation: {
                address: null,
                awards: [],
                email: 'alex@example.com',
                hobbies: [],
                id: 8,
                languages: [],
                phoneNumber: null,
                socialMediaLinks: [],
                useSameAwardsAsProfile: false,
                useSameEmailAsProfile: false,
                useSameHobbiesAsProfile: false,
                useSameLanguagesAsProfile: false,
                useSamePhoneNumberAsProfile: false,
                useSameSocialMediaLinksAsProfile: false,
            },
            photo: 'photo',
            projects: [
                {
                    description: null,
                    endDate: null,
                    id: 3,
                    isCurrent: true,
                    projectName: 'Product',
                    role: null,
                    startDate: '2025-01-02T00:00:00Z',
                    technologiesUsed: null,
                },
            ],
            resumeTemplate: 'modern-technical',
            skills: [
                {
                    description: null,
                    id: 4,
                    proficiencyLevel: null,
                    skillName: 'TypeScript',
                    yearsOfExperience: 5,
                },
            ],
            summary: 'Summary',
            title: 'Resume',
            userId: 'user-1',
            workExperiences: [
                {
                    companyName: null,
                    description: null,
                    endDate: null,
                    id: 5,
                    isCurrent: true,
                    jobTitle: 'Engineer',
                    location: null,
                    startDate: '2024-01-02T00:00:00Z',
                },
            ],
        } satisfies ResumeDetailResponse);

        expect(form.certifications[0]).toMatchObject({
            issueDate: '2025-02-03',
        });
        expect(form.education[0]).toMatchObject({
            passingYear: '2020',
            percentage: '80',
        });
        expect(form.projects[0]).toMatchObject({ startDate: '2025-01-02' });
        expect(form.skills[0]).toMatchObject({ yearsOfExperience: '5' });
        expect(form.workExperiences[0]).toMatchObject({ jobTitle: 'Engineer' });
    });

    it('routes all service operations and normalizes create and update payloads', async () => {
        const apiClient = {
            delete: vi.fn(),
            get: vi.fn(),
            post: vi.fn(),
            put: vi.fn(),
        };
        const service = createResumeService(
            apiClient as Parameters<typeof createResumeService>[0],
        );
        const values = createEmptyResumeForm('user-1');
        values.title = '  Resume title  ';
        values.summary = '  Summary  ';
        values.resumeId = 9;
        values.personalInformation.removeAddress = true;

        service.getResume(3);
        service.searchResumes({ pageNumber: 1, pageSize: 10 });
        service.deleteResume(3);
        service.deleteResumes([3, 4]);
        service.createResume(values);
        service.updateResume(9, values);

        expect(apiClient.get).toHaveBeenCalledWith('/resumes/3');
        expect(apiClient.post).toHaveBeenCalledWith('/resumes/search', {
            pageNumber: 1,
            pageSize: 10,
        });
        expect(apiClient.delete).toHaveBeenCalledWith('/resumes/3');
        expect(apiClient.post).toHaveBeenCalledWith('/resumes/delete', {
            resumeIds: [3, 4],
        });
        expect(apiClient.post).toHaveBeenCalledWith(
            '/resumes',
            expect.objectContaining({
                summary: 'Summary',
                title: 'Resume title',
            }),
        );
        expect(apiClient.put).toHaveBeenCalledWith(
            '/resumes/9',
            expect.objectContaining({
                removePersonalInformation: false,
                personalInformation: expect.objectContaining({ address: null }),
            }),
        );
    });
});
