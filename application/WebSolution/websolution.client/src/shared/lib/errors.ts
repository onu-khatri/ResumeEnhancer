import { ApiError } from '@/shared/api/api-client';

export type SharedRequestState =
    | 'authorization'
    | 'empty'
    | 'entitlement'
    | 'error'
    | 'loading'
    | 'offline'
    | 'ready';

export function getSharedRequestState(error?: unknown): SharedRequestState {
    if (!error) {
        return 'ready';
    }

    if (error instanceof ApiError) {
        if (error.status === 401 || error.status === 403) {
            return 'authorization';
        }

        if (error.status === 402) {
            return 'entitlement';
        }
    }

    if (error instanceof TypeError) {
        return 'offline';
    }

    return 'error';
}

export function getErrorMessage(error: unknown) {
    return error instanceof Error
        ? error.message
        : 'The request could not be completed.';
}
