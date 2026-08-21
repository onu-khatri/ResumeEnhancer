import type { ThemeName } from '@/app/theme-provider';

export interface ProfilePreferenceSource {
    getThemePreference: () => Promise<string | null>;
}

export function resolveThemePreference(
    localTheme: ThemeName,
    remoteTheme: string | null,
): ThemeName {
    return remoteTheme === 'light' ||
        remoteTheme === 'midnight' ||
        remoteTheme === 'forest'
        ? remoteTheme
        : localTheme;
}
