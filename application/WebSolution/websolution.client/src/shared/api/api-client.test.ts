import { afterEach, describe, expect, it, vi } from 'vitest';

import { ApiError, createApiClient } from '@/shared/api/api-client';

const fetchMock = vi.fn();
vi.stubGlobal('fetch', fetchMock);

afterEach(() => fetchMock.mockReset());

function response(
    body: unknown,
    options: { contentType?: string; ok?: boolean; status?: number } = {},
) {
    const {
        contentType = 'application/json',
        ok = true,
        status = 200,
    } = options;
    return {
        headers: new Headers({ 'content-type': contentType }),
        json: async () => body,
        ok,
        status,
        text: async () => String(body),
    } as Response;
}

describe('api client', () => {
    it('serializes JSON bodies and includes auth headers', async () => {
        fetchMock.mockResolvedValue(response({ id: 1 }));
        const client = createApiClient({
            getAccessToken: () => 'token',
            onUnauthorized: vi.fn(),
            refreshSession: async () => false,
        });
        await expect(
            client.post<{ id: number }>('/items', { name: 'Resume' }),
        ).resolves.toEqual({ id: 1 });
        const [, init] = fetchMock.mock.calls[0];
        expect(init.body).toBe('{"name":"Resume"}');
        expect(init.headers.get('Authorization')).toBe('Bearer token');
        expect(init.headers.get('Content-Type')).toBe('application/json');
        expect(init.credentials).toBe('include');
    });

    it('preserves form and string bodies and handles empty responses', async () => {
        fetchMock
            .mockResolvedValueOnce(
                response('', { contentType: 'text/plain', status: 204 }),
            )
            .mockResolvedValueOnce(
                response('done', { contentType: 'text/plain' }),
            );
        const client = createApiClient({
            getAccessToken: () => null,
            onUnauthorized: vi.fn(),
            refreshSession: async () => false,
        });
        await expect(client.put('/empty', new FormData())).resolves.toBeNull();
        await expect(client.delete('/text')).resolves.toBe('done');
        expect(fetchMock.mock.calls[0][1].headers.has('Content-Type')).toBe(
            false,
        );
    });

    it('refreshes once after unauthorized responses and reports failed authorization', async () => {
        const onUnauthorized = vi.fn();
        const refreshSession = vi
            .fn()
            .mockResolvedValueOnce(true)
            .mockResolvedValueOnce(false);
        fetchMock
            .mockResolvedValueOnce(
                response('unauthorized', {
                    contentType: 'text/plain',
                    ok: false,
                    status: 401,
                }),
            )
            .mockResolvedValueOnce(response({ ok: true }))
            .mockResolvedValueOnce(
                response('', {
                    contentType: 'text/plain',
                    ok: false,
                    status: 401,
                }),
            );
        const client = createApiClient({
            getAccessToken: () => null,
            onUnauthorized,
            refreshSession,
        });
        await expect(client.get('/refresh')).resolves.toEqual({ ok: true });
        await expect(client.get('/deny')).rejects.toMatchObject({
            message: 'The request could not be completed.',
            status: 401,
        });
        expect(refreshSession).toHaveBeenCalledTimes(2);
        expect(onUnauthorized).toHaveBeenCalledOnce();
    });

    it('throws API errors with JSON details', async () => {
        fetchMock.mockResolvedValue(
            response({ code: 'bad-request' }, { ok: false, status: 400 }),
        );
        const client = createApiClient({
            getAccessToken: () => null,
            onUnauthorized: vi.fn(),
            refreshSession: async () => false,
        });
        await expect(client.get('/bad')).rejects.toBeInstanceOf(ApiError);
        await expect(client.get('/bad')).rejects.toMatchObject({
            details: { code: 'bad-request' },
            status: 400,
        });
    });
});
