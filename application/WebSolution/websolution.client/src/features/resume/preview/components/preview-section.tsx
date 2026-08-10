import type { PropsWithChildren } from 'react';

export function PreviewSection({
  children,
  title,
}: PropsWithChildren<{ title: string }>) {
  return (
    <section>
      <h2 className="text-sm font-semibold uppercase tracking-[0.28em] text-slate-500">
        {title}
      </h2>
      <div className="mt-4">{children}</div>
    </section>
  );
}
