import { create } from 'zustand';
import { createJSONStorage, persist } from 'zustand/middleware';

import { env } from '@/config/env';
import type { ResumeFormValues } from '@/features/resume/model/types';

export interface ResumeDraftRecord {
    resumeId: number | null;
    updatedAt: string;
    values: ResumeFormValues;
}

interface ResumeDraftStore {
    clearDraft: () => void;
    draft: ResumeDraftRecord | null;
    saveDraft: (values: ResumeFormValues, resumeId: number | null) => string;
}

export const useResumeDraftStore = create<ResumeDraftStore>()(
    persist(
        (set) => ({
            clearDraft: () => set({ draft: null }),
            draft: null,
            saveDraft: (values, resumeId) => {
                const updatedAt = new Date().toISOString();
                set({
                    draft: {
                        resumeId,
                        updatedAt,
                        values,
                    },
                });

                return updatedAt;
            },
        }),
        {
            name: env.draftStorageKey,
            partialize: (state) => ({
                draft: state.draft,
            }),
            storage: createJSONStorage(() => localStorage),
        },
    ),
);
