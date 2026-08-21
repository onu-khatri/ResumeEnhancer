import { render, screen } from '@testing-library/react';
import type { ReactNode } from 'react';
import { describe, expect, it, vi } from 'vitest';

vi.mock('@/features/resume/layout/resume-shell', () => ({
    ResumeShell: ({ children }: { children: ReactNode }) => <>{children}</>,
}));

import { BuilderCardSection } from '@/features/resume/builder/components/builder-card-section';
import { BuilderSkeleton } from '@/features/resume/builder/components/builder-skeleton';
import { SectionHeading } from '@/features/resume/builder/components/section-heading';

describe('builder static components', () => {
    it('renders headings and card content', () => {
        render(
            <BuilderCardSection description="Details" title="Identity">
                <p>Content</p>
            </BuilderCardSection>,
        );
        expect(
            screen.getByRole('heading', { name: 'Identity' }),
        ).toBeInTheDocument();
        expect(screen.getByText('Content')).toBeInTheDocument();
        expect(screen.getByText('Details')).toBeInTheDocument();
    });

    it('renders standalone headings and builder skeletons', () => {
        const { rerender } = render(
            <SectionHeading description="Description" title="Section" />,
        );
        expect(
            screen.getByRole('heading', { name: 'Section' }),
        ).toBeInTheDocument();
        rerender(<BuilderSkeleton />);
        expect(
            document.querySelectorAll('.animate-pulse').length,
        ).toBeGreaterThan(0);
    });
});
