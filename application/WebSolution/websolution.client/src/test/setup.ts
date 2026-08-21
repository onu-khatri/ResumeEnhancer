import '@testing-library/jest-dom/vitest';

import { cleanup } from '@testing-library/react';
import { afterEach } from 'vitest';

class ResizeObserverMock {
    disconnect() {}
    observe() {}
    unobserve() {}
}

globalThis.ResizeObserver = ResizeObserverMock;

afterEach(() => {
    cleanup();
    localStorage.clear();
});
