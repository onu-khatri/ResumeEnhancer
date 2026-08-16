export function SectionHeading({
    description,
    title,
}: {
    description: string;
    title: string;
}) {
    return (
        <div>
            <h2 className="text-2xl font-semibold text-slate-950 dark:text-white">
                {title}
            </h2>
            <p className="mt-2 text-sm leading-6 text-slate-600 dark:text-slate-300">
                {description}
            </p>
        </div>
    );
}
