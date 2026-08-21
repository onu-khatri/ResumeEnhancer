import { describe, expect, it, vi } from 'vitest';

import {
    readStoredValue,
    removeStoredValue,
    writeStoredValue,
} from '@/shared/lib/storage';

describe('storage utilities', () => {
    it('reads, writes, and removes values when storage is available', () => {
        writeStoredValue('theme', 'forest');
        expect(readStoredValue('theme')).toBe('forest');
        removeStoredValue('theme');
        expect(readStoredValue('theme')).toBeNull();
    });

    it('returns safe fallbacks when storage methods throw', () => {
        const getItem = vi
            .spyOn(Storage.prototype, 'getItem')
            .mockImplementation(() => {
                throw new Error('blocked');
            });
        expect(readStoredValue('theme')).toBeNull();
        getItem.mockRestore();
        const setItem = vi
            .spyOn(Storage.prototype, 'setItem')
            .mockImplementation(() => {
                throw new Error('blocked');
            });
        expect(() => writeStoredValue('theme', 'forest')).not.toThrow();
        setItem.mockRestore();
        const removeItem = vi
            .spyOn(Storage.prototype, 'removeItem')
            .mockImplementation(() => {
                throw new Error('blocked');
            });
        expect(() => removeStoredValue('theme')).not.toThrow();
        removeItem.mockRestore();
    });
});
