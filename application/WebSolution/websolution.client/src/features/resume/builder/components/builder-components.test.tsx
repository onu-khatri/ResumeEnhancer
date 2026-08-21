import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it, vi } from 'vitest';

import { BuilderAlerts } from '@/features/resume/builder/components/builder-alerts';
import { BuilderSidebar } from '@/features/resume/builder/components/builder-sidebar';
import { CompactSection } from '@/features/resume/builder/components/compact-section';
import { RepeatingSection } from '@/features/resume/builder/components/repeating-section';

describe('builder components', () => {
    it('adds and removes compact-section items', async () => {
        const user = userEvent.setup();
        const append = vi.fn();
        const remove = vi.fn();
        render(
            <CompactSection
                array={{ append, fields: [{ id: 'first' }], remove }}
                onAdd={() => ({ id: 'new' })}
                title="Link"
            >
                {(index) => <p>Link {index + 1}</p>}
            </CompactSection>,
        );

        await user.click(screen.getByRole('button', { name: 'Add' }));
        await user.click(screen.getByRole('button', { name: 'Remove Link 1' }));
        expect(append).toHaveBeenCalledWith({ id: 'new' });
        expect(remove).toHaveBeenCalledWith(0);
    });

    it('renders repeating-section errors and forwards row controls', async () => {
        const user = userEvent.setup();
        const move = vi.fn();
        const onAdd = vi.fn();
        const remove = vi.fn();
        render(
            <RepeatingSection
                errors={{ root: { message: 'Add at least one item' } } as never}
                fields={[{ id: 'experience-1' }]}
                move={move}
                onAdd={onAdd}
                remove={remove}
                title="Experience"
            >
                {() => <p>Experience details</p>}
            </RepeatingSection>,
        );

        expect(screen.getByText('Add at least one item')).toBeInTheDocument();
        await user.click(screen.getByRole('button', { name: 'Add item' }));
        await user.click(
            screen.getByRole('button', { name: 'Move Experience 1 up' }),
        );
        await user.click(
            screen.getByRole('button', { name: 'Move Experience 1 down' }),
        );
        await user.click(
            screen.getByRole('button', { name: 'Remove Experience 1' }),
        );

        expect(onAdd).toHaveBeenCalledOnce();
        expect(move).toHaveBeenNthCalledWith(1, 0, -1);
        expect(move).toHaveBeenNthCalledWith(2, 0, 1);
        expect(remove).toHaveBeenCalledWith(0);
    });

    it('renders alerts and both completion and autosave states', async () => {
        const user = userEvent.setup();
        const clearDraft = vi.fn();
        render(
            <>
                <BuilderAlerts
                    draft={
                        {
                            formValues: {},
                            resumeId: 1,
                            updatedAt: new Date().toISOString(),
                        } as never
                    }
                    saveError={new Error('Could not save')}
                />
                <BuilderSidebar
                    clearDraft={clearDraft}
                    completionPercent={50}
                    lastSavedAt={new Date().toISOString()}
                    progressItems={[
                        { complete: true, label: 'Identity' },
                        { complete: false, label: 'Projects' },
                    ]}
                />
            </>,
        );

        expect(screen.getByText('Draft restored')).toBeInTheDocument();
        expect(screen.getByText('Could not save')).toBeInTheDocument();
        expect(screen.getByText('Done')).toBeInTheDocument();
        expect(screen.getByText('Pending')).toBeInTheDocument();
        expect(screen.getByText(/Last autosave/)).toBeInTheDocument();
        await user.click(
            screen.getByRole('button', { name: 'Clear local draft' }),
        );
        expect(clearDraft).toHaveBeenCalledOnce();
    });

    it('omits alerts and shows the initial autosave guidance without draft data', () => {
        render(
            <>
                <BuilderAlerts draft={null} saveError={null} />
                <BuilderSidebar
                    clearDraft={vi.fn()}
                    completionPercent={0}
                    lastSavedAt={null}
                    progressItems={[]}
                />
            </>,
        );

        expect(screen.queryByText('Draft restored')).not.toBeInTheDocument();
        expect(
            screen.getByText(
                'Autosave will begin once the form finishes hydrating.',
            ),
        ).toBeInTheDocument();
    });
});
