import type { SelectOption } from '@/shared/ui/select-field';

export interface ResumeTemplateDefinition {
    description: string;
    label: string;
    value: string;
    previewClassName: string;
}

export interface TemplateCatalogItem {
    description?: string;
    displayName: string;
    key: string;
}

const templates: ResumeTemplateDefinition[] = [
    {
        description:
            'Balanced, recruiter-friendly layout with strong readability.',
        label: 'Executive Clean',
        value: 'executive-clean',
        previewClassName: 'lg:grid-cols-[0.8fr_1.2fr]',
    },
    {
        description:
            'A denser layout for technical experience and project depth.',
        label: 'Modern Technical',
        value: 'modern-technical',
        previewClassName: 'lg:grid-cols-[0.68fr_1.32fr]',
    },
    {
        description:
            'A calmer, story-led layout for product and creative roles.',
        label: 'Narrative Portfolio',
        value: 'narrative-portfolio',
        previewClassName: 'lg:grid-cols-[1fr_1fr]',
    },
];

export function getResumeTemplate(
    value: string | null | undefined,
): ResumeTemplateDefinition {
    return (
        templates.find((template) => template.value === value) ?? templates[0]
    );
}

export function getResumeTemplateOptions(): SelectOption[] {
    return templates.map(({ description, label, value }) => ({
        description,
        label,
        value,
    }));
}

export function mapTemplateCatalog(
    items: readonly TemplateCatalogItem[],
): ResumeTemplateDefinition[] {
    return items.map((item) => ({
        description:
            item.description ?? 'Template supplied by the template catalog.',
        label: item.displayName,
        value: item.key,
        previewClassName: 'lg:grid-cols-[0.8fr_1.2fr]',
    }));
}
