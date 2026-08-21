export function normalizeText(value: string | null | undefined) {
    return value?.trim() ?? '';
}

export function normalizeOptionalText(value: string | null | undefined) {
    const normalized = normalizeText(value);
    return normalized || null;
}

export function uniqueTextValues(values: readonly string[]) {
    return Array.from(
        new Set(values.map(normalizeText).filter((value) => value.length > 0)),
    );
}
