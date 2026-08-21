import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';

import { ResumeTemplateRenderer } from '@/features/resume/templates/resume-template-renderer';

describe('resume template renderer', () => {
    it.each([
        ['executive-clean', 'Executive'],
        ['modern-technical', 'Technical'],
        ['narrative-portfolio', 'Narrative'],
        ['unsupported-template', 'Fallback'],
    ])('preserves shared content for %s', (template, label) => {
        render(
            <ResumeTemplateRenderer
                hero={<p>{label} hero</p>}
                main={<p>{label} main</p>}
                sidebar={<p>{label} sidebar</p>}
                template={template}
            />,
        );

        expect(screen.getByText(`${label} hero`)).toBeInTheDocument();
        expect(screen.getByText(`${label} main`)).toBeInTheDocument();
        expect(screen.getByText(`${label} sidebar`)).toBeInTheDocument();
    });
});
