import { describe, expect, it } from 'vitest';

import { resolveThemePreference } from '@/shared/lib/profile-preference';

describe('profile preference resolution', () => {
    it('uses supported remote preferences and preserves local fallback otherwise', () => {
        expect(resolveThemePreference('light', 'midnight')).toBe('midnight');
        expect(resolveThemePreference('forest', null)).toBe('forest');
        expect(resolveThemePreference('forest', 'unsupported')).toBe('forest');
    });
});
