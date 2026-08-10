import { PlusIcon } from '@heroicons/react/24/outline';

import { Button } from '@/shared/ui/button';
import { InputField } from '@/shared/ui/form-field';

export function DashboardToolbar({
  onCreateResume,
  onSearchTextChange,
  searchText,
}: {
  onCreateResume: () => void;
  onSearchTextChange: (value: string) => void;
  searchText: string;
}) {
  return (
    <div className="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
      <div className="max-w-xl flex-1">
        <InputField
          label="Search resumes"
          onChange={(event) => onSearchTextChange(event.target.value)}
          placeholder="Search by title or summary"
          value={searchText}
        />
      </div>
      <div className="flex gap-3">
        <Button onClick={onCreateResume}>
          <PlusIcon className="h-4 w-4" />
          New resume
        </Button>
      </div>
    </div>
  );
}
