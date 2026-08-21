import { render, screen } from '@testing-library/react';
import type { ReactNode } from 'react';
import { describe, expect, it, vi } from 'vitest';

vi.mock('@/features/resume/layout/resume-shell', () => ({
    ResumeShell: ({ children }: { children: ReactNode }) => <>{children}</>,
}));

import { PreviewSection } from '@/features/resume/preview/components/preview-section';
import { PreviewSkeleton } from '@/features/resume/preview/components/preview-skeleton';
import { PreviewHero } from '@/features/resume/preview/components/preview-hero';
import { PreviewMainColumn } from '@/features/resume/preview/components/preview-main-column';
import { PreviewSidebar } from '@/features/resume/preview/components/preview-sidebar';
import { TimelineCard } from '@/features/resume/preview/components/timeline-card';
import type { ResumeDetailResponse } from '@/features/resume/model/types';

function createResume(
    overrides: Partial<ResumeDetailResponse> = {},
): ResumeDetailResponse {
    return {
        app_CreateDate: '2026-01-15T00:00:00.000Z',
        app_UpdateDate: '2026-02-01T00:00:00.000Z',
        app_Version: [1],
        certifications: [],
        education: [],
        id: 1,
        personalInformation: null,
        photo: null,
        projects: [],
        resumeTemplate: 'executive-clean',
        skills: [],
        summary: null,
        title: 'Platform Engineer',
        userId: 'user-1',
        workExperiences: [],
        ...overrides,
    };
}

describe('preview static components', () => {
    it('renders sections and timeline details', () => {
        render(
            <PreviewSection title="Skills">
                <p>React</p>
            </PreviewSection>,
        );
        expect(
            screen.getByRole('heading', { name: 'Skills' }),
        ).toBeInTheDocument();
        render(
            <TimelineCard
                eyebrow="2026"
                subtitle="Company"
                text="Impact"
                title="Engineer"
            />,
        );
        expect(screen.getByText('Impact')).toBeInTheDocument();
    });

    it('renders preview skeleton blocks', () => {
        render(<PreviewSkeleton />);
        expect(
            document.querySelectorAll('.animate-pulse').length,
        ).toBeGreaterThan(0);
    });

    it('renders populated preview content from one resume model', () => {
        const resume = createResume({
            certifications: [
                {
                    certificationName: 'Cloud Architect',
                    credentialId: null,
                    credentialUrl: null,
                    description: 'Cloud delivery',
                    expirationDate: null,
                    id: 4,
                    issueDate: '2024-03-01',
                    issuingOrganization: 'Cloud Academy',
                },
            ],
            education: [
                {
                    city: null,
                    degree: 'BSc Computer Science',
                    description: 'First class',
                    grade: null,
                    id: 3,
                    institution: 'University',
                    isCurrent: false,
                    passingYear: 2020,
                    percentage: null,
                    state: null,
                },
            ],
            personalInformation: {
                address: {
                    city: 'Pune',
                    country: 'India',
                    id: 2,
                    line1: '1 Main Street',
                    line2: null,
                    postalCode: null,
                    state: 'Maharashtra',
                },
                awards: [],
                email: 'alex@example.com',
                hobbies: [],
                id: 1,
                languages: [],
                phoneNumber: '+91 555 0100',
                socialMediaLinks: [
                    {
                        displayName: 'Profile',
                        id: 5,
                        platform: 'LinkedIn',
                        url: 'https://example.com/alex',
                    },
                ],
                useSameAwardsAsProfile: false,
                useSameEmailAsProfile: false,
                useSameHobbiesAsProfile: false,
                useSameLanguagesAsProfile: false,
                useSamePhoneNumberAsProfile: false,
                useSameSocialMediaLinksAsProfile: false,
            },
            projects: [
                {
                    description: 'Reduced deployment time.',
                    endDate: '2025-12-01',
                    id: 6,
                    isCurrent: false,
                    projectName: 'Delivery platform',
                    role: null,
                    startDate: '2025-01-01',
                    technologiesUsed: 'React',
                },
            ],
            skills: [
                {
                    description: null,
                    id: 7,
                    proficiencyLevel: null,
                    skillName: 'TypeScript',
                    yearsOfExperience: null,
                },
            ],
            summary: 'I build dependable product systems.',
            workExperiences: [
                {
                    companyName: 'Acme',
                    description: 'Led delivery.',
                    endDate: null,
                    id: 8,
                    isCurrent: true,
                    jobTitle: 'Senior Engineer',
                    location: null,
                    startDate: '2023-01-01',
                },
            ],
        });

        render(
            <>
                <PreviewHero
                    contactDetails={['alex@example.com']}
                    resume={resume}
                />
                <PreviewMainColumn resume={resume} />
                <PreviewSidebar resume={resume} />
            </>,
        );

        expect(
            screen.getByText('I build dependable product systems.'),
        ).toBeInTheDocument();
        expect(screen.getByText('Senior Engineer')).toBeInTheDocument();
        expect(screen.getByText('Delivery platform')).toBeInTheDocument();
        expect(screen.getByText('BSc Computer Science')).toBeInTheDocument();
        expect(screen.getByText('Cloud Architect')).toBeInTheDocument();
        expect(screen.getByText('TypeScript')).toBeInTheDocument();
        expect(
            screen.getByRole('link', { name: 'LinkedIn Profile' }),
        ).toHaveAttribute('href', 'https://example.com/alex');
    });

    it('renders preview fallback messages when optional content is absent', () => {
        const resume = createResume({
            app_UpdateDate: null,
            resumeTemplate: null,
        });
        render(
            <>
                <PreviewHero contactDetails={[]} resume={resume} />
                <PreviewMainColumn resume={resume} />
                <PreviewSidebar resume={resume} />
            </>,
        );

        expect(screen.queryByText('Platform Engineer')).toBeInTheDocument();
        expect(
            screen.getByText('No work experience has been added yet.'),
        ).toBeInTheDocument();
        expect(
            screen.getByText('No projects have been added yet.'),
        ).toBeInTheDocument();
        expect(
            screen.getByText('No education records yet.'),
        ).toBeInTheDocument();
        expect(
            screen.getByText('No certifications have been added yet.'),
        ).toBeInTheDocument();
        expect(
            screen.getByText('No contact details available.'),
        ).toBeInTheDocument();
        expect(screen.getByText('No skills added yet.')).toBeInTheDocument();
        expect(
            screen.getByText('No social links available.'),
        ).toBeInTheDocument();
    });
});
