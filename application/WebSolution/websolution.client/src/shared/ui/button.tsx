import { Slot } from '@radix-ui/react-slot';
import { cva, type VariantProps } from 'class-variance-authority';
import type { ButtonHTMLAttributes } from 'react';

import { cn } from '@/shared/lib/cn';

const buttonVariants = cva(
    'inline-flex items-center justify-center gap-2 rounded-2xl text-sm font-semibold transition duration-200 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-teal-700/70 focus-visible:ring-offset-2 focus-visible:ring-offset-white disabled:pointer-events-none disabled:opacity-50 dark:focus-visible:ring-offset-slate-950',
    {
        defaultVariants: {
            size: 'md',
            variant: 'primary',
        },
        variants: {
            size: {
                icon: 'h-11 w-11',
                md: 'h-11 px-5',
                sm: 'h-9 px-4 text-xs',
            },
            variant: {
                ghost: 'border border-transparent bg-transparent text-slate-700 hover:bg-slate-100 hover:text-slate-950 dark:text-slate-200 dark:hover:bg-slate-800',
                outline:
                    'border border-slate-300 bg-white text-slate-800 shadow-sm hover:border-teal-700 hover:text-slate-950 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100',
                primary:
                    'bg-slate-950 text-white shadow-[0_16px_32px_rgba(15,23,42,0.18)] hover:-translate-y-0.5 hover:bg-teal-700 dark:bg-teal-500 dark:text-slate-950 dark:hover:bg-teal-400',
                subtle: 'border border-teal-200 bg-teal-50 text-teal-900 hover:bg-teal-100 dark:border-teal-400/20 dark:bg-teal-400/10 dark:text-teal-200',
            },
        },
    },
);

type ButtonProps = ButtonHTMLAttributes<HTMLButtonElement> &
    VariantProps<typeof buttonVariants> & {
        asChild?: boolean;
    };

export function Button({
    asChild,
    className,
    size,
    variant,
    ...props
}: ButtonProps) {
    const Component = asChild ? Slot : 'button';

    return (
        <Component
            className={cn(buttonVariants({ className, size, variant }))}
            {...props}
        />
    );
}
