import { render, screen } from '@testing-library/react';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import { describe, expect, it, vi } from 'vitest';

import { ProtectedRoute } from '@/routes/protected-route';

const { useAuth } = vi.hoisted(() => ({ useAuth: vi.fn() }));
vi.mock('@/features/auth/auth-context', () => ({ useAuth }));

function renderRoute() {
    return render(
        <MemoryRouter initialEntries={['/app']}>
            <Routes>
                <Route element={<ProtectedRoute />} path="/app">
                    <Route element={<p>Workspace</p>} index />
                </Route>
                <Route element={<p>Login</p>} path="/login" />
            </Routes>
        </MemoryRouter>,
    );
}

describe('ProtectedRoute', () => {
    it('waits for hydration', () => {
        useAuth.mockReturnValue({ isAuthenticated: false, isReady: false });
        const { container } = renderRoute();
        expect(container).toBeEmptyDOMElement();
    });

    it('redirects unauthenticated users and renders authenticated children', () => {
        useAuth.mockReturnValue({ isAuthenticated: false, isReady: true });
        const firstRender = renderRoute();
        expect(screen.getByText('Login')).toBeInTheDocument();
        firstRender.unmount();
        useAuth.mockReturnValue({ isAuthenticated: true, isReady: true });
        renderRoute();
        expect(screen.getByText('Workspace')).toBeInTheDocument();
    });
});
