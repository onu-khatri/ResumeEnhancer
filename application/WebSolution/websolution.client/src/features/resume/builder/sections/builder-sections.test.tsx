import { render, screen } from '@testing-library/react';
import type { ReactNode } from 'react';
import { describe, expect, it, vi } from 'vitest';

vi.mock('@/features/resume/builder/components/repeating-section', () => ({
    RepeatingSection: ({
        children,
        title,
    }: {
        children: (field: { id: string }, index: number) => ReactNode;
        title: string;
    }) => (
        <section>
            <h2>{title}</h2>
            {children({ id: `${title}-item` }, 0)}
        </section>
    ),
}));

vi.mock('@/features/resume/builder/components/compact-section', () => ({
    CompactSection: ({
        children,
        title,
    }: {
        children: (index: number) => ReactNode;
        title: string;
    }) => (
        <section>
            <h2>{title}</h2>
            {children(0)}
        </section>
    ),
}));

import { CertificationsSection } from '@/features/resume/builder/sections/certifications-section';
import { EducationSection } from '@/features/resume/builder/sections/education-section';
import { IdentitySection } from '@/features/resume/builder/sections/identity-section';
import { PersonalDetailsSection } from '@/features/resume/builder/sections/personal-details-section';
import { PersonalExtrasSection } from '@/features/resume/builder/sections/personal-extras-section';
import { ProjectsSection } from '@/features/resume/builder/sections/projects-section';
import { SkillsSection } from '@/features/resume/builder/sections/skills-section';
import { SocialLinksSection } from '@/features/resume/builder/sections/social-links-section';
import { WorkExperienceSection } from '@/features/resume/builder/sections/work-experience-section';

const form = {
    formState: { errors: {} },
    register: (name: string) => ({ name }),
    setValue: vi.fn(),
} as never;

const repeatingArray = {
    append: vi.fn(),
    fields: [{ id: 'item-1' }],
    move: vi.fn(),
    remove: vi.fn(),
} as never;

const compactArray = {
    append: vi.fn(),
    fields: [{ id: 'item-1' }],
    remove: vi.fn(),
};

describe('builder section adapters', () => {
    it('renders identity and personal detail bindings', () => {
        render(
            <>
                <IdentitySection
                    form={form}
                    values={{ resumeTemplate: 'executive-clean' } as never}
                />
                <PersonalDetailsSection form={form} />
            </>,
        );

        expect(
            screen.getByRole('textbox', { name: /Resume title/ }),
        ).toHaveAttribute('name', 'title');
        expect(
            screen.getByRole('textbox', { name: /Professional summary/ }),
        ).toHaveAttribute('name', 'summary');
        expect(screen.getByLabelText('Email')).toHaveAttribute(
            'name',
            'personalInformation.email',
        );
        expect(screen.getByLabelText('Postal code')).toHaveAttribute(
            'name',
            'personalInformation.address.postalCode',
        );
    });

    it('renders every repeated section field group', () => {
        render(
            <>
                <SkillsSection array={repeatingArray} form={form} />
                <CertificationsSection array={repeatingArray} form={form} />
                <EducationSection array={repeatingArray} form={form} />
                <ProjectsSection array={repeatingArray} form={form} />
                <SocialLinksSection array={repeatingArray} form={form} />
                <WorkExperienceSection array={repeatingArray} form={form} />
                <PersonalExtrasSection
                    awards={compactArray}
                    form={form}
                    hobbies={compactArray}
                    languages={compactArray}
                />
            </>,
        );

        expect(screen.getByLabelText('Skill')).toHaveAttribute(
            'name',
            'skills.0.skillName',
        );
        expect(screen.getByLabelText('Certification name')).toHaveAttribute(
            'name',
            'certifications.0.certificationName',
        );
        expect(screen.getByLabelText('Degree')).toHaveAttribute(
            'name',
            'education.0.degree',
        );
        expect(screen.getByLabelText('Project name')).toHaveAttribute(
            'name',
            'projects.0.projectName',
        );
        expect(screen.getByLabelText('Platform')).toHaveAttribute(
            'name',
            'personalInformation.socialMediaLinks.0.platform',
        );
        expect(screen.getByLabelText('Job title')).toHaveAttribute(
            'name',
            'workExperiences.0.jobTitle',
        );
        expect(screen.getByLabelText('Current role')).toHaveAttribute(
            'name',
            'workExperiences.0.isCurrent',
        );
        expect(screen.getByLabelText('Award')).toHaveAttribute(
            'name',
            'personalInformation.awards.0.awardName',
        );
        expect(screen.getByLabelText('Language')).toHaveAttribute(
            'name',
            'personalInformation.languages.0.languageName',
        );
        expect(screen.getByLabelText('Hobby')).toHaveAttribute(
            'name',
            'personalInformation.hobbies.0.hobbyName',
        );
    });
});
