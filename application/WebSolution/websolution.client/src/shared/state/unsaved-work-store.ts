import { create } from 'zustand';

interface UnsavedWorkState {
    hasUnsavedWork: boolean;
    setHasUnsavedWork: (value: boolean) => void;
}

export const useUnsavedWorkStore = create<UnsavedWorkState>((set) => ({
    hasUnsavedWork: false,
    setHasUnsavedWork: (hasUnsavedWork) => set({ hasUnsavedWork }),
}));
