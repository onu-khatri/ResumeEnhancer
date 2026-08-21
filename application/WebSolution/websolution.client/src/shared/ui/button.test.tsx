import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';

import { Button } from '@/shared/ui/button';

describe('Button', () => {
    it('renders supported variants, sizes, disabled state, and child slots', () => {
        const { rerender } = render(
            <Button disabled variant="outline">
                Save
            </Button>,
        );
        expect(screen.getByRole('button', { name: 'Save' })).toBeDisabled();
        rerender(
            <Button size="icon" variant="ghost">
                Icon
            </Button>,
        );
        expect(screen.getByRole('button', { name: 'Icon' })).toHaveClass(
            'h-11',
        );
        rerender(
            <Button asChild variant="subtle">
                <a href="/resume">Resume</a>
            </Button>,
        );
        expect(screen.getByRole('link', { name: 'Resume' })).toHaveAttribute(
            'href',
            '/resume',
        );
    });
});
