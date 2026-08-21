export interface StorageAdapter {
    getItem: (key: string) => string | null;
    removeItem: (key: string) => void;
    setItem: (key: string, value: string) => void;
}

function getStorage(): StorageAdapter | null {
    try {
        return typeof window === 'undefined' ? null : window.localStorage;
    } catch {
        return null;
    }
}

export function readStoredValue(key: string): string | null {
    try {
        return getStorage()?.getItem(key) ?? null;
    } catch {
        return null;
    }
}

export function removeStoredValue(key: string) {
    try {
        getStorage()?.removeItem(key);
    } catch {
        // Storage access is optional; callers retain their in-memory state.
    }
}

export function writeStoredValue(key: string, value: string) {
    try {
        getStorage()?.setItem(key, value);
    } catch {
        // Storage access is optional; callers retain their in-memory state.
    }
}
