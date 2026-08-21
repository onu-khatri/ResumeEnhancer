import { describe, expect, it } from 'vitest';

import {
    getResumeTemplate,
    getResumeTemplateOptions,
    mapTemplateCatalog,
} from '@/features/resume/templates/template-registry';

describe('template registry', () => {
    it('resolves supported and fallback templates', () => {
        expect(getResumeTemplate('modern-technical').label).toBe(
            'Modern Technical',
        );
        expect(getResumeTemplate('unknown').value).toBe('executive-clean');
        expect(getResumeTemplate(null).value).toBe('executive-clean');
    });

    it('provides dropdown options and maps future catalog entries', () => {
        expect(getResumeTemplateOptions()).toHaveLength(3);
        expect(
            mapTemplateCatalog([
                { displayName: 'Module template', key: 'module-template' },
                {
                    description: 'Catalog description',
                    displayName: 'Detailed template',
                    key: 'detailed-template',
                },
            ]),
        ).toEqual([
            expect.objectContaining({
                description: 'Template supplied by the template catalog.',
                value: 'module-template',
            }),
            expect.objectContaining({
                description: 'Catalog description',
                value: 'detailed-template',
            }),
        ]);
    });
});
