import { env } from '@/config/env';

export class ApiError extends Error {
  public readonly details?: unknown;
  public readonly status: number;

  constructor(
    message: string,
    status: number,
    details?: unknown,
  ) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.details = details;
  }
}

interface ApiClientOptions {
  getAccessToken: () => string | null;
  onUnauthorized: () => void;
  refreshSession: () => Promise<boolean>;
}

type RequestOptions = Omit<RequestInit, 'body'> & {
  body?: BodyInit | Record<string, unknown> | null;
  retried?: boolean;
};

async function parseResponse(response: Response) {
  const contentType = response.headers.get('content-type') ?? '';

  if (response.status === 204) {
    return null;
  }

  if (contentType.includes('application/json')) {
    return (await response.json()) as unknown;
  }

  return await response.text();
}

export function createApiClient(options: ApiClientOptions) {
  async function request<T>(path: string, init: RequestOptions = {}): Promise<T> {
    const headers = new Headers(init.headers);
    const accessToken = options.getAccessToken();

    if (!headers.has('Content-Type') && init.body && !(init.body instanceof FormData)) {
      headers.set('Content-Type', 'application/json');
    }

    if (accessToken) {
      headers.set('Authorization', `Bearer ${accessToken}`);
    }

    const requestBody =
      init.body && !(init.body instanceof FormData) && typeof init.body !== 'string'
        ? JSON.stringify(init.body)
        : init.body;

    const response = await fetch(`${env.apiBaseUrl}${path}`, {
      ...init,
      body: requestBody,
      credentials: 'include',
      headers,
    });

    if (response.status === 401 && !init.retried) {
      const didRefresh = await options.refreshSession();
      if (didRefresh) {
        return request<T>(path, { ...init, retried: true });
      }

      options.onUnauthorized();
    }

    const payload = await parseResponse(response);

    if (!response.ok) {
      throw new ApiError(
        typeof payload === 'string' && payload
          ? payload
          : 'The request could not be completed.',
        response.status,
        payload,
      );
    }

    return payload as T;
  }

  return {
    delete: <T>(path: string, init?: RequestOptions) =>
      request<T>(path, { ...init, method: 'DELETE' }),
    get: <T>(path: string, init?: RequestOptions) =>
      request<T>(path, { ...init, method: 'GET' }),
    post: <T>(path: string, body?: RequestOptions['body'], init?: RequestOptions) =>
      request<T>(path, { ...init, body, method: 'POST' }),
    put: <T>(path: string, body?: RequestOptions['body'], init?: RequestOptions) =>
      request<T>(path, { ...init, body, method: 'PUT' }),
  };
}
