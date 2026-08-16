import { PlusIcon, TrashIcon } from '@heroicons/react/24/outline';
import type { ReactNode } from 'react';

import { Button } from '@/shared/ui/button';
import { Card } from '@/shared/ui/card';

export function CompactSection<TItem>({
    array,
    children,
    onAdd,
    title,
}: {
    array: {
        append: (value: TItem) => void;
        fields: Array<{ id: string }>;
        remove: (index: number) => void;
    };
    children: (index: number) => ReactNode;
    onAdd: () => TItem;
    title: string;
}) {
    return (
        <Card className="rounded-[2rem] p-0">
            <div className="flex items-center justify-between border-b border-slate-200 px-5 py-4 dark:border-slate-800">
                <h2 className="text-lg font-semibold text-slate-950 dark:text-white">
                    {title}
                </h2>
                <Button
                    onClick={() => array.append(onAdd())}
                    size="sm"
                    type="button"
                    variant="outline"
                >
                    <PlusIcon className="h-4 w-4" />
                    Add
                </Button>
            </div>
            <div className="space-y-4 px-5 py-5">
                {array.fields.map((field, index) => (
                    <Card
                        key={field.id}
                        className="rounded-[1.5rem] border-slate-200/90 bg-slate-50/70 p-4 shadow-none dark:border-slate-800 dark:bg-slate-950"
                    >
                        <div className="mb-4 flex justify-end">
                            <Button
                                onClick={() => array.remove(index)}
                                size="icon"
                                type="button"
                                variant="ghost"
                            >
                                <TrashIcon className="h-4 w-4" />
                            </Button>
                        </div>
                        <div className="space-y-4">{children(index)}</div>
                    </Card>
                ))}
            </div>
        </Card>
    );
}
