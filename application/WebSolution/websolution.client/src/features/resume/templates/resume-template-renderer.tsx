import type { ReactNode } from 'react';

import { getResumeTemplate } from '@/features/resume/templates/template-registry';
import { Card } from '@/shared/ui/card';

interface ResumeTemplateRendererProps {
    hero: ReactNode;
    main: ReactNode;
    sidebar: ReactNode;
    template: string | null;
}

export function ResumeTemplateRenderer({
    hero,
    main,
    sidebar,
    template,
}: ResumeTemplateRendererProps) {
    switch (getResumeTemplate(template).value) {
        case 'modern-technical':
            return (
                <ModernTechnicalTemplate
                    hero={hero}
                    main={main}
                    sidebar={sidebar}
                />
            );
        case 'narrative-portfolio':
            return (
                <NarrativePortfolioTemplate
                    hero={hero}
                    main={main}
                    sidebar={sidebar}
                />
            );
        default:
            return (
                <ExecutiveCleanTemplate
                    hero={hero}
                    main={main}
                    sidebar={sidebar}
                />
            );
    }
}

function ExecutiveCleanTemplate({
    hero,
    main,
    sidebar,
}: Omit<ResumeTemplateRendererProps, 'template'>) {
    return (
        <Card className="overflow-hidden rounded-[2rem] border-white/60 p-0 dark:border-white/10">
            <div>{hero}</div>
            <div className="grid gap-8 px-8 py-8 sm:px-10 lg:grid-cols-[0.8fr_1.2fr]">
                <aside>{sidebar}</aside>
                <main>{main}</main>
            </div>
        </Card>
    );
}

function ModernTechnicalTemplate({
    hero,
    main,
    sidebar,
}: Omit<ResumeTemplateRendererProps, 'template'>) {
    return (
        <Card className="overflow-hidden rounded-xl border-[var(--color-accent)]/30 p-0">
            <div className="border-b-4 border-[var(--color-accent)]">
                {hero}
            </div>
            <div className="grid gap-8 px-8 py-8 sm:px-10 lg:grid-cols-[0.68fr_1.32fr]">
                <aside className="border-r border-[var(--color-border)] pr-6">
                    {sidebar}
                </aside>
                <main>{main}</main>
            </div>
        </Card>
    );
}

function NarrativePortfolioTemplate({
    hero,
    main,
    sidebar,
}: Omit<ResumeTemplateRendererProps, 'template'>) {
    return (
        <Card className="overflow-hidden rounded-[1.25rem] p-0">
            <div className="bg-[var(--surface-subtle)]">{hero}</div>
            <div className="grid gap-10 px-8 py-8 sm:px-10 lg:grid-cols-[1fr_1fr]">
                <main>{main}</main>
                <aside className="lg:border-l lg:border-[var(--color-border)] lg:pl-8">
                    {sidebar}
                </aside>
            </div>
        </Card>
    );
}
