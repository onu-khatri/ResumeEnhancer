import {
    ArrowDownIcon,
    ArrowUpIcon,
    PlusIcon,
    TrashIcon,
} from '@heroicons/react/24/outline';
import type { ReactNode } from 'react';
import type { FieldErrors } from 'react-hook-form';

import type { ResumeFormValues } from '@/features/resume/model/types';
import { Button } from '@/shared/ui/button';
import { Card } from '@/shared/ui/card';

export function RepeatingSection({
    children,
    errors,
    fields,
    move,
    onAdd,
    remove,
    title,
}: {
    children: (field: { id: string }, index: number) => ReactNode;
    errors: FieldErrors<ResumeFormValues> | undefined;
    fields: Array<{ id: string }>;
    move: (from: number, to: number) => void;
    onAdd: () => void;
    remove: (index: number) => void;
    title: string;
}) {
    return (
        <Card className="rounded-[2rem] p-0">
            <div className="flex items-center justify-between border-b border-slate-200 px-6 py-5 dark:border-slate-800">
                <div>
                    <h2 className="text-2xl font-semibold text-slate-950 dark:text-white">
                        {title}
                    </h2>
                    {errors && 'root' in errors && errors.root ? (
                        <p className="mt-2 text-sm text-rose-600">
                            {errors.root.message as string}
                        </p>
                    ) : null}
                </div>
                <Button onClick={onAdd} variant="outline">
                    <PlusIcon className="h-4 w-4" />
                    Add item
                </Button>
            </div>
            <div className="space-y-5 px-6 py-6">
                {fields.map((field, index) => (
                    <Card
                        key={field.id}
                        className="rounded-[1.75rem] border-slate-200/90 bg-slate-50/70 shadow-none dark:border-slate-800 dark:bg-slate-950"
                    >
                        <div className="mb-5 flex items-center justify-between">
                            <p className="text-sm font-semibold tracking-[0.24em] text-slate-500 uppercase">
                                {title} {index + 1}
                            </p>
                            <div className="flex gap-2">
                                <IconButton
                                    icon={<ArrowUpIcon className="h-4 w-4" />}
                                    onClick={() => move(index, index - 1)}
                                />
                                <IconButton
                                    icon={<ArrowDownIcon className="h-4 w-4" />}
                                    onClick={() => move(index, index + 1)}
                                />
                                <IconButton
                                    icon={<TrashIcon className="h-4 w-4" />}
                                    onClick={() => remove(index)}
                                />
                            </div>
                        </div>
                        {children(field, index)}
                    </Card>
                ))}
            </div>
        </Card>
    );
}

function IconButton({
    icon,
    onClick,
}: {
    icon: ReactNode;
    onClick: () => void;
}) {
    return (
        <Button onClick={onClick} size="icon" type="button" variant="ghost">
            {icon}
        </Button>
    );
}
