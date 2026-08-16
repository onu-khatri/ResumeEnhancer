import { fileURLToPath, URL } from 'node:url';

import react from '@vitejs/plugin-react';
import tailwindcss from '@tailwindcss/vite';
import { env } from 'node:process';
import { defineConfig } from 'vite';

const target = env.ASPNETCORE_URLS
    ? env.ASPNETCORE_URLS.split(';')[0]
    : env.ASPNETCORE_HTTPS_PORT
      ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
      : 'http://localhost:5274';

export default defineConfig({
    plugins: [react(), tailwindcss()],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url)),
        },
    },
    server: {
        proxy: {
            '^/api': {
                target,
                secure: false,
            },
        },
        port: Number.parseInt(env.DEV_SERVER_PORT ?? '56866', 10),
    },
});
