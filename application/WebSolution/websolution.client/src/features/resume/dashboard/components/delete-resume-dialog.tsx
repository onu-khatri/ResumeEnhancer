import {
  Dialog,
  DialogBackdrop,
  DialogPanel,
  DialogTitle,
} from '@headlessui/react';

import type { ResumeListItemResponse } from '@/features/resume/model/types';
import { Button } from '@/shared/ui/button';

export function DeleteResumeDialog({
  isDeleting,
  onClose,
  onConfirm,
  resume,
}: {
  isDeleting: boolean;
  onClose: () => void;
  onConfirm: () => void;
  resume: ResumeListItemResponse | null;
}) {
  return (
    <Dialog className="relative z-50" open={resume !== null} onClose={onClose}>
      <DialogBackdrop className="fixed inset-0 bg-slate-950/45 backdrop-blur-sm" />
      <div className="fixed inset-0 flex items-center justify-center p-4">
        <DialogPanel className="w-full max-w-md rounded-[2rem] border border-white/10 bg-white p-6 shadow-2xl dark:bg-slate-900">
          <DialogTitle className="text-xl font-semibold text-slate-950 dark:text-white">
            Delete this resume?
          </DialogTitle>
          <p className="mt-3 text-sm leading-6 text-slate-600 dark:text-slate-300">
            {resume
              ? `This will permanently remove "${resume.title}" from the resume dashboard.`
              : 'This action will permanently remove the selected resume.'}
          </p>
          <div className="mt-6 flex justify-end gap-3">
            <Button onClick={onClose} variant="outline">
              Cancel
            </Button>
            <Button onClick={onConfirm} disabled={isDeleting}>
              {isDeleting ? 'Deleting...' : 'Delete resume'}
            </Button>
          </div>
        </DialogPanel>
      </div>
    </Dialog>
  );
}
