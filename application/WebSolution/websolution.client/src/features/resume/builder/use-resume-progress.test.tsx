import { renderHook } from '@testing-library/react';
import { describe, expect, it } from 'vitest';

import { useResumeProgress } from '@/features/resume/builder/use-resume-progress';
import { createEmptyResumeForm } from '@/features/resume/model/types';

describe('useResumeProgress', () => {
    it('reports every completed resume section', () => {
        const values = createEmptyResumeForm('user-1');
        values.title = 'Platform Engineer';
        values.personalInformation.email = 'alex@example.com';
        values.summary = 'A'.repeat(80);
        values.skills[0].skillName = 'TypeScript';
        values.education[0].degree = 'Computer Science';
        values.workExperiences[0].jobTitle = 'Engineer';
        values.projects[0].projectName = 'Resume Enhancer';
        values.certifications[0].certificationName = 'Cloud Architect';

        const { result } = renderHook(() =>
            useResumeProgress(values, 'user-1'),
        );

        expect(result.current.completionPercent).toBe(100);
        expect(result.current.progressItems).toEqual(
            expect.arrayContaining([
                { complete: true, label: 'Title' },
                { complete: true, label: 'Certifications' },
            ]),
        );
    });

    it('uses empty defaults and reports incomplete sections when values are absent', () => {
        const { result } = renderHook(() =>
            useResumeProgress(undefined as never, undefined),
        );

        expect(result.current.completionPercent).toBe(0);
        expect(
            result.current.progressItems.every((item) => !item.complete),
        ).toBe(true);
    });
});
