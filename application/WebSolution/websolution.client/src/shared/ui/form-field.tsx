import type {
  InputHTMLAttributes,
  PropsWithChildren,
  TextareaHTMLAttributes,
} from 'react';
import { forwardRef } from 'react';

import { cn } from '@/shared/lib/cn';

interface FieldShellProps {
  description?: string;
  error?: string;
  label: string;
  required?: boolean;
}

function FieldShell({
  children,
  description,
  error,
  label,
  required,
}: PropsWithChildren<FieldShellProps>) {
  return (
    <label className="grid gap-2">
      <span className="text-sm font-semibold text-slate-800 dark:text-slate-100">
        {label}
        {required ? <span className="ml-1 text-rose-500">*</span> : null}
      </span>
      {children}
      {description ? (
        <span className="text-xs leading-5 text-slate-600 dark:text-slate-300">
          {description}
        </span>
      ) : null}
      {error ? <span className="text-xs font-medium text-rose-700">{error}</span> : null}
    </label>
  );
}

interface InputFieldProps extends InputHTMLAttributes<HTMLInputElement> {
  description?: string;
  error?: string;
  label: string;
}

export const InputField = forwardRef<HTMLInputElement, InputFieldProps>(
  ({ className, description, error, label, required, ...props }, ref) => (
    <FieldShell
      description={description}
      error={error}
      label={label}
      required={required}
    >
      <input
        ref={ref}
        className={cn(
          'h-12 rounded-2xl border border-slate-300 bg-white px-4 text-sm text-slate-950 shadow-sm transition placeholder:text-slate-500 focus:border-teal-700 focus:outline-none focus:ring-4 focus:ring-teal-700/12 dark:border-slate-700 dark:bg-slate-950 dark:text-white dark:placeholder:text-slate-400',
          error && 'border-rose-500 focus:border-rose-600 focus:ring-rose-600/12',
          className,
        )}
        {...props}
      />
    </FieldShell>
  ),
);

InputField.displayName = 'InputField';

interface TextareaFieldProps
  extends TextareaHTMLAttributes<HTMLTextAreaElement> {
  description?: string;
  error?: string;
  label: string;
}

export const TextareaField = forwardRef<HTMLTextAreaElement, TextareaFieldProps>(
  ({ className, description, error, label, required, ...props }, ref) => (
    <FieldShell
      description={description}
      error={error}
      label={label}
      required={required}
    >
      <textarea
        ref={ref}
        className={cn(
          'min-h-28 rounded-3xl border border-slate-300 bg-white px-4 py-3 text-sm text-slate-950 shadow-sm transition placeholder:text-slate-500 focus:border-teal-700 focus:outline-none focus:ring-4 focus:ring-teal-700/12 dark:border-slate-700 dark:bg-slate-950 dark:text-white dark:placeholder:text-slate-400',
          error && 'border-rose-500 focus:border-rose-600 focus:ring-rose-600/12',
          className,
        )}
        {...props}
      />
    </FieldShell>
  ),
);

TextareaField.displayName = 'TextareaField';
