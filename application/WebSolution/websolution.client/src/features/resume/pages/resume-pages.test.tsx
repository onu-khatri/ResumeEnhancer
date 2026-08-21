import { render, screen } from '@testing-library/react';
import type { ReactNode } from 'react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter } from 'react-router-dom';
import { describe, expect, it, vi } from 'vitest';

const mocks = vi.hoisted(() => ({
    builder: {} as Record<string, unknown>,
    dashboard: {} as Record<string, unknown>,
    previewQuery: {} as Record<string, unknown>,
    navigate: vi.fn(),
}));

vi.mock('react-router-dom', async () => {
    const actual =
        await vi.importActual<Record<string, unknown>>('react-router-dom');
    return {
        ...actual,
        useLocation: () => ({ state: null }),
        useNavigate: () => mocks.navigate,
    };
});
vi.mock('@/features/auth/auth-context', () => ({
    useAuth: () => ({ session: { resumeId: 1 } }),
}));
vi.mock('@/features/resume/layout/resume-shell', () => ({
    ResumeShell: ({
        actions,
        children,
    }: {
        actions?: ReactNode;
        children: ReactNode;
    }) => (
        <>
            {actions}
            {children}
        </>
    ),
}));
vi.mock('@/shared/ui/status', () => ({
    ErrorState: ({ onRetry }: { onRetry?: () => void }) => (
        <button onClick={onRetry}>Retry</button>
    ),
    EmptyState: ({ title, action }: { title: string; action?: ReactNode }) => (
        <>
            {title}
            {action}
        </>
    ),
    InlineAlert: ({ title }: { title: string }) => <p>{title}</p>,
    SkeletonBlock: () => <div data-testid="skeleton" />,
}));
vi.mock('@/features/resume/builder/components/builder-skeleton', () => ({
    BuilderSkeleton: () => <p>Builder loading</p>,
}));
vi.mock('@/features/resume/builder/components/builder-alerts', () => ({
    BuilderAlerts: () => <div />,
}));
vi.mock('@/features/resume/builder/components/builder-sidebar', () => ({
    BuilderSidebar: () => <div />,
}));
vi.mock('@/features/resume/builder/sections/identity-section', () => ({
    IdentitySection: () => <div />,
}));
vi.mock('@/features/resume/builder/sections/personal-details-section', () => ({
    PersonalDetailsSection: () => <div />,
}));
vi.mock('@/features/resume/builder/sections/personal-extras-section', () => ({
    PersonalExtrasSection: () => <div />,
}));
vi.mock('@/features/resume/builder/sections/certifications-section', () => ({
    CertificationsSection: () => <div />,
}));
vi.mock('@/features/resume/builder/sections/education-section', () => ({
    EducationSection: () => <div />,
}));
vi.mock('@/features/resume/builder/sections/projects-section', () => ({
    ProjectsSection: () => <div />,
}));
vi.mock('@/features/resume/builder/sections/skills-section', () => ({
    SkillsSection: () => <div />,
}));
vi.mock('@/features/resume/builder/sections/social-links-section', () => ({
    SocialLinksSection: () => <div />,
}));
vi.mock('@/features/resume/builder/sections/work-experience-section', () => ({
    WorkExperienceSection: () => <div />,
}));
vi.mock('@/features/resume/builder/use-resume-builder-controller', () => ({
    useResumeBuilderController: () => mocks.builder,
}));
vi.mock('@/features/resume/builder/use-resume-progress', () => ({
    useResumeProgress: () => ({ completionPercent: 0, progressItems: [] }),
}));
vi.mock('@/shared/hooks/use-unsaved-work-warning', () => ({
    useUnsavedWorkWarning: vi.fn(),
}));
vi.mock('@/features/resume/dashboard/use-resume-dashboard', () => ({
    useResumeDashboard: () => mocks.dashboard,
}));
vi.mock('@/features/resume/dashboard/components/dashboard-pagination', () => ({
    DashboardPagination: () => <div />,
}));
vi.mock('@/features/resume/dashboard/components/dashboard-toolbar', () => ({
    DashboardToolbar: () => <div />,
}));
vi.mock('@/features/resume/dashboard/components/delete-resume-dialog', () => ({
    DeleteResumeDialog: () => <div />,
}));
vi.mock('@/features/resume/dashboard/components/resume-dashboard-list', () => ({
    ResumeDashboardList: () => <p>Resume list</p>,
}));
vi.mock('@/features/resume/hooks/use-resume-query', () => ({
    useResumeQuery: () => mocks.previewQuery,
}));
vi.mock('@/features/resume/preview/components/preview-skeleton', () => ({
    PreviewSkeleton: () => <p>Preview loading</p>,
}));
vi.mock('@/features/resume/templates/resume-template-renderer', () => ({
    ResumeTemplateRenderer: () => <p>Rendered resume</p>,
}));
vi.mock('@/features/resume/preview/components/preview-hero', () => ({
    PreviewHero: () => <div />,
}));
vi.mock('@/features/resume/preview/components/preview-main-column', () => ({
    PreviewMainColumn: () => <div />,
}));
vi.mock('@/features/resume/preview/components/preview-sidebar', () => ({
    PreviewSidebar: () => <div />,
}));

import { ResumeBuilderPage } from '@/features/resume/pages/resume-builder-page';
import { ResumeDashboardPage } from '@/features/resume/pages/resume-dashboard-page';
import { ResumePreviewPage } from '@/features/resume/pages/resume-preview-page';

function builderState(overrides: Record<string, unknown> = {}) {
    return {
        arrays: {},
        canHydrate: true,
        clearDraft: vi.fn(),
        form: { formState: { isDirty: false }, getValues: () => 'user-1' },
        lastSavedAt: null,
        matchingDraft: null,
        resumeQuery: { isError: false, isPending: false },
        retryResumeLoad: vi.fn(),
        saveResume: vi.fn(),
        submitResume: { error: null, isPending: false },
        values: {},
        ...overrides,
    };
}

describe('resume pages', () => {
    it('renders builder loading, error retry, and save states', async () => {
        mocks.builder = builderState({
            canHydrate: false,
            resumeQuery: { isError: false, isPending: true },
        });
        const { rerender } = render(<ResumeBuilderPage />);
        expect(screen.getByText('Builder loading')).toBeInTheDocument();
        const retry = vi.fn();
        mocks.builder = builderState({
            canHydrate: false,
            resumeQuery: { isError: true, isPending: false },
            retryResumeLoad: retry,
        });
        rerender(<ResumeBuilderPage />);
        await userEvent
            .setup()
            .click(screen.getByRole('button', { name: 'Retry' }));
        expect(retry).toHaveBeenCalledOnce();
        const saveResume = vi.fn();
        mocks.builder = builderState({
            saveResume,
            submitResume: { error: new Error('Save failed'), isPending: true },
        });
        rerender(<ResumeBuilderPage />);
        await userEvent
            .setup()
            .click(screen.getByRole('button', { name: 'Saving...' }));
        expect(saveResume).toHaveBeenCalledOnce();
    });

    it('renders dashboard loading, error, content, and empty states', () => {
        mocks.dashboard = {
            createResume: vi.fn(),
            deleteResume: { isError: false, isPending: false },
            resumeSearch: { isError: false, isPending: true },
            searchText: '',
            setPageNumber: vi.fn(),
            setSearchText: vi.fn(),
            selectResume: vi.fn(),
        };
        const { rerender } = render(<ResumeDashboardPage />);
        expect(screen.getAllByTestId('skeleton')).not.toHaveLength(0);
        mocks.dashboard = {
            ...mocks.dashboard,
            resumeSearch: { isError: true, isPending: false, refetch: vi.fn() },
        };
        rerender(<ResumeDashboardPage />);
        expect(
            screen.getByRole('button', { name: 'Retry' }),
        ).toBeInTheDocument();
        mocks.dashboard = {
            ...mocks.dashboard,
            resumeSearch: {
                data: { items: [{ id: 1 }], totalCount: 1 },
                isError: false,
                isPending: false,
            },
        };
        rerender(<ResumeDashboardPage />);
        expect(screen.getByText('Resume list')).toBeInTheDocument();
        mocks.dashboard = {
            ...mocks.dashboard,
            resumeSearch: {
                data: { items: [] },
                isError: false,
                isPending: false,
            },
        };
        rerender(<ResumeDashboardPage />);
        expect(screen.getByText('No resumes found')).toBeInTheDocument();
    });

    it('renders preview loading, error, empty, and content states', () => {
        mocks.previewQuery = { isError: false, isPending: true };
        const { rerender } = render(
            <MemoryRouter>
                <ResumePreviewPage />
            </MemoryRouter>,
        );
        expect(screen.getByText('Preview loading')).toBeInTheDocument();
        mocks.previewQuery = {
            isError: true,
            isPending: false,
            refetch: vi.fn(),
        };
        rerender(
            <MemoryRouter>
                <ResumePreviewPage />
            </MemoryRouter>,
        );
        expect(
            screen.getByRole('button', { name: 'Retry' }),
        ).toBeInTheDocument();
        mocks.previewQuery = {
            data: undefined,
            isError: false,
            isPending: false,
        };
        rerender(
            <MemoryRouter>
                <ResumePreviewPage />
            </MemoryRouter>,
        );
        expect(screen.getByText('Nothing to preview')).toBeInTheDocument();
        mocks.previewQuery = {
            data: { personalInformation: null, resumeTemplate: null },
            isError: false,
            isPending: false,
        };
        rerender(
            <MemoryRouter>
                <ResumePreviewPage />
            </MemoryRouter>,
        );
        expect(screen.getByText('Rendered resume')).toBeInTheDocument();
    });
});
