import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it, vi } from 'vitest';

import { DashboardToolbar } from '@/features/resume/dashboard/components/dashboard-toolbar';
import { DeleteResumeDialog } from '@/features/resume/dashboard/components/delete-resume-dialog';
import { ResumeDashboardList } from '@/features/resume/dashboard/components/resume-dashboard-list';
import type { ResumeListItemResponse } from '@/features/resume/model/types';

const resume: ResumeListItemResponse = {
    app_CreateDate: '2026-01-15T00:00:00.000Z',
    app_UpdateDate: null,
    certificationCount: 1,
    educationCount: 2,
    id: 24,
    photo: null,
    projectCount: 4,
    resumeTemplate: null,
    skillCount: 3,
    summary: '   ',
    title: 'Platform engineer',
    userId: 'user-1',
    workExperienceCount: 5,
};

describe('dashboard components', () => {
    it('forwards toolbar input and creation actions', async () => {
        const user = userEvent.setup();
        const onCreateResume = vi.fn();
        const onSearchTextChange = vi.fn();
        render(
            <DashboardToolbar
                onCreateResume={onCreateResume}
                onSearchTextChange={onSearchTextChange}
                searchText="engineer"
            />,
        );

        const search = screen.getByRole('textbox', {
            name: 'Search resumes',
        });
        expect(search).toHaveValue('engineer');
        await user.type(search, 's');
        await user.click(screen.getByRole('button', { name: 'New resume' }));

        expect(onSearchTextChange).toHaveBeenCalledWith('engineers');
        expect(onCreateResume).toHaveBeenCalledOnce();
    });

    it('renders dashboard fallbacks and forwards row actions', async () => {
        const user = userEvent.setup();
        const onDelete = vi.fn();
        const onEdit = vi.fn();
        const onView = vi.fn();
        render(
            <ResumeDashboardList
                items={[resume]}
                onDelete={onDelete}
                onEdit={onEdit}
                onView={onView}
            />,
        );

        expect(screen.getByText('Resume')).toBeInTheDocument();
        expect(screen.getByText('No summary added yet.')).toBeInTheDocument();
        expect(screen.getByText('2 edu • 5 exp')).toBeInTheDocument();
        await user.click(screen.getByRole('button', { name: 'View' }));
        await user.click(screen.getByRole('button', { name: 'Edit' }));
        await user.click(screen.getByRole('button', { name: 'Delete' }));

        expect(onView).toHaveBeenCalledWith(24);
        expect(onEdit).toHaveBeenCalledWith(24);
        expect(onDelete).toHaveBeenCalledWith(resume);
    });

    it('supports cancel, confirm, and deleting dialog states', async () => {
        const user = userEvent.setup();
        const onClose = vi.fn();
        const onConfirm = vi.fn();
        const { rerender } = render(
            <DeleteResumeDialog
                isDeleting={false}
                onClose={onClose}
                onConfirm={onConfirm}
                resume={resume}
            />,
        );

        expect(screen.getByRole('dialog')).toHaveTextContent(
            'permanently remove "Platform engineer"',
        );
        await user.click(screen.getByRole('button', { name: 'Cancel' }));
        await user.click(screen.getByRole('button', { name: 'Delete resume' }));
        expect(onClose).toHaveBeenCalledOnce();
        expect(onConfirm).toHaveBeenCalledOnce();

        rerender(
            <DeleteResumeDialog
                isDeleting
                onClose={onClose}
                onConfirm={onConfirm}
                resume={resume}
            />,
        );
        expect(
            screen.getByRole('button', { name: 'Deleting...' }),
        ).toBeDisabled();
    });
});
