import { Card } from '@/shared/ui/card';

export function TimelineCard({
  eyebrow,
  subtitle,
  text,
  title,
}: {
  eyebrow: string;
  subtitle: string;
  text: string;
  title: string;
}) {
  return (
    <Card className="rounded-[1.75rem] border-slate-300/70 bg-slate-50/85 shadow-none dark:border-slate-800 dark:bg-slate-900/80">
      <p className="text-xs font-semibold uppercase tracking-[0.24em] text-teal-800 dark:text-teal-300">
        {eyebrow}
      </p>
      <h3 className="mt-3 text-lg font-semibold text-slate-950 dark:text-white">
        {title}
      </h3>
      <p className="mt-1 text-sm font-medium text-slate-700 dark:text-slate-300">
        {subtitle}
      </p>
      <p className="mt-4 text-sm leading-7 text-slate-700 dark:text-slate-300">
        {text}
      </p>
    </Card>
  );
}
