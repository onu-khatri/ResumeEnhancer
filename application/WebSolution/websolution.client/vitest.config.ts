import { fileURLToPath, URL } from 'node:url';

import react from '@vitejs/plugin-react';
import { defineConfig } from 'vitest/config';

export default defineConfig({
    plugins: [react()],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url)),
        },
    },
    test: {
        coverage: {
            all: true,
            include: ['src/**/*.{ts,tsx}'],
            provider: 'v8',
            reporter: ['text', 'html'],
            thresholds: {
                branches: 92,
                functions: 92,
                lines: 92,
                statements: 92,
            },
        },
        environment: 'jsdom',
        globals: true,
        setupFiles: ['./src/test/setup.ts'],
    },
});
