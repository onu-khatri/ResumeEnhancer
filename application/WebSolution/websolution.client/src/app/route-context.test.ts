import { describe, expect, it } from 'vitest';

import { routeContexts } from '@/app/route-context';

describe('route contexts', () => {
    it('declares public and authenticated shell metadata', () => {
        expect(routeContexts.login).toMatchObject({
            shell: 'public',
            title: 'Sign in',
        });
        expect(routeContexts.dashboard).toMatchObject({
            shell: 'authenticated',
            title: 'Resume dashboard',
        });
        expect(routeContexts.builder).toMatchObject({
            warnsOnUnsavedWork: true,
        });
    });
});
