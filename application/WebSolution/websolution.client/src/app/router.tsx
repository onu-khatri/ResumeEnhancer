/* eslint-disable react-refresh/only-export-components */
import { createBrowserRouter, Navigate } from 'react-router-dom';

import { useAuth } from '@/features/auth/auth-context';
import { LoginPage } from '@/pages/login-page';
import { NotFoundPage } from '@/pages/not-found-page';
import { ProtectedRoute } from '@/routes/protected-route';
import { ResumeBuilderPage } from '@/features/resume/pages/resume-builder-page';
import { ResumeDashboardPage } from '@/features/resume/pages/resume-dashboard-page';
import { ResumePreviewPage } from '@/features/resume/pages/resume-preview-page';
import { routeContexts } from '@/app/route-context';

function HomeGate() {
    const { isAuthenticated } = useAuth();

    return (
        <Navigate
            replace
            to={isAuthenticated ? '/app/resume/dashboard' : '/login'}
        />
    );
}

export const appRouter = createBrowserRouter([
    {
        path: '/',
        element: <HomeGate />,
    },
    {
        path: '/login',
        element: <LoginPage />,
        handle: routeContexts.login,
    },
    {
        path: '/app',
        element: <ProtectedRoute />,
        children: [
            {
                path: 'resume',
                element: <Navigate replace to="/app/resume/dashboard" />,
            },
            {
                path: 'resume/dashboard',
                element: <ResumeDashboardPage />,
                handle: routeContexts.dashboard,
            },
            {
                path: 'resume/builder',
                element: <ResumeBuilderPage />,
                handle: routeContexts.builder,
            },
            {
                path: 'resume/preview',
                element: <ResumePreviewPage />,
                handle: routeContexts.preview,
            },
        ],
    },
    {
        path: '*',
        element: <NotFoundPage />,
    },
]);
