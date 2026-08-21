import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it } from 'vitest';

import { ThemeProvider, themeOptions, useTheme } from '@/app/theme-provider';

function ThemeHarness() {
    const { setTheme, syncProfilePreference, theme } = useTheme();
    return (
        <>
            <p>{theme}</p>
            <button onClick={() => setTheme('midnight')} type="button">
                Midnight
            </button>
            <button
                onClick={() =>
                    void syncProfilePreference({
                        getThemePreference: async () => 'forest',
                    })
                }
                type="button"
            >
                Sync
            </button>
        </>
    );
}

describe('theme provider', () => {
    it('uses a persisted default, applies themes, and synchronizes a valid profile preference', async () => {
        const user = userEvent.setup();
        localStorage.setItem('resume-enhancer.theme', 'light');
        render(
            <ThemeProvider>
                <ThemeHarness />
            </ThemeProvider>,
        );

        expect(themeOptions).toEqual(['light', 'midnight', 'forest']);
        expect(screen.getByText('light')).toBeInTheDocument();
        await user.click(screen.getByRole('button', { name: 'Midnight' }));
        expect(document.documentElement.dataset.theme).toBe('midnight');
        expect(document.body).toHaveClass('dark');
        await user.click(screen.getByRole('button', { name: 'Sync' }));
        await waitFor(() =>
            expect(screen.getByText('forest')).toBeInTheDocument(),
        );
        expect(localStorage.getItem('resume-enhancer.theme')).toBe('forest');
    });
});
