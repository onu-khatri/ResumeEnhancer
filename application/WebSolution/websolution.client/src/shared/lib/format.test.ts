import { describe, expect, it, vi } from 'vitest';

import {
    formatDate,
    formatDateRange,
    formatRelativeTime,
} from '@/shared/lib/format';

describe('format utilities', () => {
    it('formats valid and invalid dates and ranges', () => {
        expect(formatDate('2026-08-22')).toBe('Aug 2026');
        expect(formatDate('invalid')).toBe('Present');
        expect(formatDate(null)).toBe('Present');
        expect(formatDateRange(null, null, false)).toBe('Start date - Present');
        expect(formatDateRange('2026-01-01', '2026-02-01', true)).toBe(
            'Jan 2026 - Present',
        );
    });

    it('formats relative minute, hour, and day differences', () => {
        vi.useFakeTimers();
        vi.setSystemTime(new Date('2026-08-22T12:00:00Z'));
        expect(formatRelativeTime('2026-08-22T11:59:00Z')).toBe('1 minute ago');
        expect(formatRelativeTime('2026-08-22T10:00:00Z')).toBe('2 hours ago');
        expect(formatRelativeTime('2026-08-20T12:00:00Z')).toBe('2 days ago');
        vi.useRealTimers();
    });
});
