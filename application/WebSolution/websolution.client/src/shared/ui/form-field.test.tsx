import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';

import { InputField, TextareaField } from '@/shared/ui/form-field';

describe('form fields', () => {
    it('renders input metadata and error state', () => {
        render(
            <InputField
                description="Use a clear title"
                error="Required"
                label="Title"
                required
            />,
        );
        expect(screen.getByLabelText(/Title/)).toBeRequired();
        expect(screen.getByText('Use a clear title')).toBeInTheDocument();
        expect(screen.getByText('Required')).toBeInTheDocument();
    });

    it('renders text areas without optional metadata', () => {
        render(<TextareaField label="Summary" />);
        expect(screen.getByLabelText('Summary')).toBeInTheDocument();
    });
});
