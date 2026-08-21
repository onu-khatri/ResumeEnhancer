import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it, vi } from 'vitest';

import { SelectField } from '@/shared/ui/select-field';

describe('SelectField', () => {
    it('renders fallback selection, descriptions, errors, and change events', async () => {
        const user = userEvent.setup();
        const onChange = vi.fn();
        render(
            <SelectField
                error="Choose one"
                label="Template"
                onChange={onChange}
                options={[
                    {
                        description: 'Primary layout',
                        label: 'Executive',
                        value: 'executive',
                    },
                    { label: 'Technical', value: 'technical' },
                ]}
                value="missing"
            />,
        );
        expect(screen.getByText('Executive')).toBeInTheDocument();
        await user.click(screen.getByRole('button', { name: /Template/ }));
        expect(screen.getByText('Primary layout')).toBeInTheDocument();
        await user.click(screen.getByText('Technical'));
        expect(onChange).toHaveBeenCalledWith('technical');
        expect(screen.getByText('Choose one')).toBeInTheDocument();
    });
});
