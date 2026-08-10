import { useEffect, useState } from 'react';

import type { ResumeFormValues } from '@/features/resume/model/types';
import { useResumeDraftStore } from '@/features/resume/state/resume-draft-store';

export function useResumeDraft(
  values: ResumeFormValues,
  resumeId: number | null,
  enabled: boolean,
) {
  const [lastSavedAt, setLastSavedAt] = useState<string | null>(null);
  const clearDraft = useResumeDraftStore((state) => state.clearDraft);
  const saveDraft = useResumeDraftStore((state) => state.saveDraft);

  useEffect(() => {
    if (!enabled) {
      return;
    }

    const timeoutId = window.setTimeout(() => {
      const updatedAt = saveDraft(values, resumeId);
      setLastSavedAt(updatedAt);
    }, 500);

    return () => window.clearTimeout(timeoutId);
  }, [enabled, resumeId, saveDraft, values]);

  return {
    clearDraft: () => {
      clearDraft();
      setLastSavedAt(null);
    },
    lastSavedAt,
  };
}
