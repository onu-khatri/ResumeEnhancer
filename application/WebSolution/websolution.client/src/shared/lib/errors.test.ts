import { describe, expect, it } from 'vitest';

import { ApiError } from '@/shared/api/api-client';
import { getErrorMessage, getSharedRequestState } from '@/shared/lib/errors';

describe('shared request-state mapping', () => {
    it('maps each module-facing error condition to a presentation state', () => {
        expect(getSharedRequestState()).toBe('ready');
        expect(getSharedRequestState(new ApiError('Denied', 401))).toBe(
            'authorization',
        );
        expect(getSharedRequestState(new ApiError('Forbidden', 403))).toBe(
            'authorization',
        );
        expect(getSharedRequestState(new ApiError('Upgrade', 402))).toBe(
            'entitlement',
        );
        expect(getSharedRequestState(new TypeError('Network failed'))).toBe(
            'offline',
        );
        expect(getSharedRequestState(new Error('Unexpected'))).toBe('error');
    });

    it('normalizes explicit and unknown error messages', () => {
        expect(getErrorMessage(new Error('Module failure'))).toBe(
            'Module failure',
        );
        expect(getErrorMessage({ message: 'Ignored' })).toBe(
            'The request could not be completed.',
        );
    });
});
