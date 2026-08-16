import type { PropsWithChildren } from 'react';

import { Card } from '@/shared/ui/card';

import { SectionHeading } from '@/features/resume/builder/components/section-heading';

export function BuilderCardSection({
    children,
    description,
    title,
}: PropsWithChildren<{ description: string; title: string }>) {
    return (
        <Card className="rounded-[2rem] p-0">
            <div className="border-b border-slate-200 px-6 py-5 dark:border-slate-800">
                <SectionHeading description={description} title={title} />
            </div>
            <div className="px-6 py-6">{children}</div>
        </Card>
    );
}
