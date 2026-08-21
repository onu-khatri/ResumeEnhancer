/* eslint-disable react-refresh/only-export-components */
import {
    createContext,
    useCallback,
    useContext,
    useEffect,
    useMemo,
    useState,
} from 'react';
import type { PropsWithChildren } from 'react';

import { readStoredValue, writeStoredValue } from '@/shared/lib/storage';
import {
    resolveThemePreference,
    type ProfilePreferenceSource,
} from '@/shared/lib/profile-preference';

export const themeOptions = ['light', 'midnight', 'forest'] as const;
export type ThemeName = (typeof themeOptions)[number];

const themeStorageKey = 'resume-enhancer.theme';

interface ThemeContextValue {
    syncProfilePreference: (source: ProfilePreferenceSource) => Promise<void>;
    setTheme: (theme: ThemeName) => void;
    theme: ThemeName;
}

const ThemeContext = createContext<ThemeContextValue | null>(null);

function isThemeName(value: string | null): value is ThemeName {
    return value !== null && themeOptions.includes(value as ThemeName);
}

function getInitialTheme(): ThemeName {
    const storedTheme = readStoredValue(themeStorageKey);
    return isThemeName(storedTheme) ? storedTheme : 'light';
}

export function ThemeProvider({ children }: PropsWithChildren) {
    const [theme, setTheme] = useState<ThemeName>(getInitialTheme);

    useEffect(() => {
        document.documentElement.dataset.theme = theme;
        document.body.classList.toggle('dark', theme === 'midnight');
        writeStoredValue(themeStorageKey, theme);
    }, [theme]);

    const syncProfilePreference = useCallback(
        async (source: ProfilePreferenceSource) => {
            const remoteTheme = await source.getThemePreference();
            setTheme((currentTheme) =>
                resolveThemePreference(currentTheme, remoteTheme),
            );
        },
        [],
    );

    const value = useMemo(
        () => ({ setTheme, syncProfilePreference, theme }),
        [syncProfilePreference, theme],
    );
    return (
        <ThemeContext.Provider value={value}>{children}</ThemeContext.Provider>
    );
}

export function useTheme() {
    const context = useContext(ThemeContext);
    if (!context) {
        throw new Error('useTheme must be used within ThemeProvider.');
    }

    return context;
}
