import { describe, expect, it } from 'vitest';

import { queryClient } from '@/app/query-client';

describe('query client', () => {
    it('provides the application query client', () => {
        expect(queryClient.getDefaultOptions()).toBeDefined();
    });
});
