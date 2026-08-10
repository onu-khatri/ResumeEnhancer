function normalizeDate(date?: string | null) {
  if (!date) {
    return null;
  }

  const parsedDate = new Date(date);
  return Number.isNaN(parsedDate.getTime()) ? null : parsedDate;
}

export function formatDate(date?: string | null, options?: Intl.DateTimeFormatOptions) {
  const parsedDate = normalizeDate(date);
  if (!parsedDate) {
    return 'Present';
  }

  return new Intl.DateTimeFormat('en-US', {
    month: 'short',
    year: 'numeric',
    ...options,
  }).format(parsedDate);
}

export function formatDateRange(
  startDate?: string | null,
  endDate?: string | null,
  isCurrent?: boolean,
) {
  const start = startDate ? formatDate(startDate) : 'Start date';
  const end = isCurrent ? 'Present' : formatDate(endDate);
  return `${start} - ${end}`;
}

export function formatRelativeTime(date: string) {
  const formatter = new Intl.RelativeTimeFormat('en', { numeric: 'auto' });
  const deltaMinutes = Math.round(
    (new Date(date).getTime() - Date.now()) / (1000 * 60),
  );

  if (Math.abs(deltaMinutes) < 60) {
    return formatter.format(deltaMinutes, 'minute');
  }

  const deltaHours = Math.round(deltaMinutes / 60);
  if (Math.abs(deltaHours) < 24) {
    return formatter.format(deltaHours, 'hour');
  }

  return formatter.format(Math.round(deltaHours / 24), 'day');
}
