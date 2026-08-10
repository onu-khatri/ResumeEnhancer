import type { ResumeDraftRecord } from '@/features/resume/state/resume-draft-store';
import { formatRelativeTime } from '@/shared/lib/format';
import { InlineAlert } from '@/shared/ui/status';

export function BuilderAlerts({
  draft,
  saveError,
}: {
  draft: ResumeDraftRecord | null;
  saveError: Error | null;
}) {
  return (
    <>
      {draft?.updatedAt ? (
        <InlineAlert
          message={`A local draft from ${formatRelativeTime(draft.updatedAt)} was restored automatically.`}
          title="Draft restored"
        />
      ) : null}
      {saveError ? (
        <InlineAlert
          message={saveError.message}
          title="Save failed"
        />
      ) : null}
    </>
  );
}
