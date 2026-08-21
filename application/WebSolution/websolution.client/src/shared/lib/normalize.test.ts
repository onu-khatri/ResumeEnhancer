import { describe, expect, it } from 'vitest';

import {
    normalizeOptionalText,
    normalizeText,
    uniqueTextValues,
} from '@/shared/lib/normalize';

describe('normalization utilities', () => {
    it('normalizes nullable values', () => {
        expect(normalizeText('  resume  ')).toBe('resume');
        expect(normalizeText(null)).toBe('');
        expect(normalizeOptionalText('   ')).toBeNull();
        expect(normalizeOptionalText(' value ')).toBe('value');
    });

    it('deduplicates normalized non-empty values', () => {
        expect(
            uniqueTextValues([' React ', '', 'React', 'TypeScript']),
        ).toEqual(['React', 'TypeScript']);
    });
});
