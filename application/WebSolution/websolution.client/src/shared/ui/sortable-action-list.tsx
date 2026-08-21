import type { ReactNode } from 'react';

export interface SortableColumn<T> {
    cell: (item: T) => ReactNode;
    header: string;
    key: string;
    sortable?: boolean;
}

export function SortableActionList<T extends { id: string | number }>({
    columns,
    items,
    onSort,
    renderActions,
    sort,
}: {
    columns: readonly SortableColumn<T>[];
    items: readonly T[];
    onSort: (key: string) => void;
    renderActions: (item: T) => ReactNode;
    sort?: { direction: 'ascending' | 'descending'; key: string };
}) {
    return (
        <div className="max-h-96 overflow-auto rounded-lg border border-slate-200 dark:border-slate-800">
            <table className="w-full border-collapse text-left text-sm">
                <thead className="sticky top-0 bg-slate-100 text-slate-800 dark:bg-slate-900 dark:text-slate-100">
                    <tr>
                        {columns.map((column) => (
                            <th
                                key={column.key}
                                aria-sort={
                                    sort?.key === column.key
                                        ? sort.direction
                                        : 'none'
                                }
                                className="px-4 py-3 font-semibold"
                            >
                                {column.sortable ? (
                                    <button
                                        className="hover:text-teal-700"
                                        onClick={() => onSort(column.key)}
                                        type="button"
                                    >
                                        {column.header}
                                    </button>
                                ) : (
                                    column.header
                                )}
                            </th>
                        ))}
                        <th className="px-4 py-3 font-semibold">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {items.map((item) => (
                        <tr
                            key={item.id}
                            className="border-t border-slate-200 dark:border-slate-800"
                        >
                            {columns.map((column) => (
                                <td key={column.key} className="px-4 py-3">
                                    {column.cell(item)}
                                </td>
                            ))}
                            <td className="px-4 py-3">{renderActions(item)}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}
