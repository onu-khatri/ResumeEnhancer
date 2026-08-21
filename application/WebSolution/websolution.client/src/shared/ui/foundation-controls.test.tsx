import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it, vi } from 'vitest';

import { Autocomplete } from '@/shared/ui/autocomplete';
import { CheckboxField } from '@/shared/ui/checkbox-field';
import { Pagination } from '@/shared/ui/pagination';
import { RadioGroup } from '@/shared/ui/radio-group';
import { RemovablePill } from '@/shared/ui/removable-pill';
import { RequestState } from '@/shared/ui/request-state';
import { SortableActionList } from '@/shared/ui/sortable-action-list';

describe('foundation controls', () => {
    it('handles checkbox, radio, autocomplete, removable-pill, and pagination interactions', async () => {
        const user = userEvent.setup();
        const onChange = vi.fn();
        const onRemove = vi.fn();
        const onPageChange = vi.fn();
        render(
            <>
                <CheckboxField label="Publish" />
                <RadioGroup
                    label="Mode"
                    onChange={onChange}
                    options={[{ label: 'Basic', value: 'basic' }]}
                    value=""
                />
                <Autocomplete
                    label="Skill"
                    onChange={onChange}
                    options={['React', 'React']}
                    value=""
                />
                <RemovablePill label="React" onRemove={onRemove} />
                <Pagination
                    hasNextPage
                    hasPreviousPage={false}
                    onPageChange={onPageChange}
                    pageNumber={2}
                    totalCount={20}
                    totalPages={3}
                />
            </>,
        );
        await user.click(screen.getByLabelText('Publish'));
        await user.click(screen.getByLabelText('Basic'));
        await user.type(screen.getByLabelText('Skill'), 'Re');
        await user.click(screen.getByLabelText('Remove React'));
        await user.click(screen.getByRole('button', { name: 'Next' }));
        expect(onChange).toHaveBeenCalledWith('basic');
        expect(onChange).toHaveBeenCalledWith('R');
        expect(onRemove).toHaveBeenCalledOnce();
        expect(onPageChange).toHaveBeenCalledWith(3);
        expect(screen.getByRole('button', { name: 'Previous' })).toBeDisabled();
    });

    it('renders sortable list actions and every request state', async () => {
        const user = userEvent.setup();
        const onSort = vi.fn();
        const onRetry = vi.fn();
        const { rerender } = render(
            <SortableActionList
                columns={[
                    {
                        cell: (item: { id: number; name: string }) => item.name,
                        header: 'Name',
                        key: 'name',
                        sortable: true,
                    },
                ]}
                items={[{ id: 1, name: 'Resume' }]}
                onSort={onSort}
                renderActions={() => <button type="button">Open</button>}
                sort={{ direction: 'ascending', key: 'name' }}
            />,
        );
        await user.click(screen.getByRole('button', { name: 'Name' }));
        expect(onSort).toHaveBeenCalledWith('name');
        expect(
            screen.getByRole('columnheader', { name: 'Name' }),
        ).toHaveAttribute('aria-sort', 'ascending');
        for (const state of [
            'loading',
            'empty',
            'authorization',
            'entitlement',
            'offline',
            'error',
            'ready',
        ] as const) {
            rerender(
                <RequestState
                    errorMessage="Failure"
                    onRetry={onRetry}
                    state={state}
                >
                    <p>Ready</p>
                </RequestState>,
            );
        }
        expect(screen.getByText('Ready')).toBeInTheDocument();
    });

    it('renders optional descriptions and alternate control states', async () => {
        const user = userEvent.setup();
        const onPageChange = vi.fn();
        render(
            <>
                <CheckboxField
                    description="Visible to collaborators"
                    label="Share"
                />
                <RadioGroup
                    label="Layout"
                    onChange={vi.fn()}
                    options={[
                        {
                            description: 'Recommended layout',
                            label: 'Modern',
                            value: 'modern',
                        },
                        { label: 'Classic', value: 'classic' },
                    ]}
                    value="modern"
                />
                <Pagination
                    hasNextPage={false}
                    hasPreviousPage
                    onPageChange={onPageChange}
                    pageNumber={2}
                    totalCount={0}
                    totalPages={0}
                />
                <SortableActionList
                    columns={[
                        {
                            cell: (item: { id: number; name: string }) =>
                                item.name,
                            header: 'Name',
                            key: 'name',
                            sortable: false,
                        },
                    ]}
                    items={[{ id: 2, name: 'Resume' }]}
                    onSort={vi.fn()}
                    renderActions={() => <button type="button">Open</button>}
                />
                <RequestState state="error">
                    <p>Ready</p>
                </RequestState>
            </>,
        );

        expect(
            screen.getByText('Visible to collaborators'),
        ).toBeInTheDocument();
        expect(screen.getByText('Recommended layout')).toBeInTheDocument();
        expect(screen.getByLabelText('Modern')).toBeChecked();
        expect(
            screen.getByText('Showing page 2 of 1 with 0 total results.'),
        ).toBeInTheDocument();
        expect(screen.getByRole('button', { name: 'Next' })).toBeDisabled();
        expect(
            screen.getByRole('columnheader', { name: 'Name' }),
        ).toHaveAttribute('aria-sort', 'none');
        expect(
            screen.getByText('The request could not be completed.'),
        ).toBeInTheDocument();

        await user.click(screen.getByRole('button', { name: 'Previous' }));
        expect(onPageChange).toHaveBeenCalledWith(1);
    });
});
