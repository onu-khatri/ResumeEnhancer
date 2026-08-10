import {
  Dialog,
  DialogBackdrop,
  DialogPanel,
  DialogTitle,
} from '@headlessui/react';
import {
  ArrowLeftStartOnRectangleIcon,
  RectangleStackIcon,
  PencilSquareIcon,
  PresentationChartBarIcon,
} from '@heroicons/react/24/outline';
import { useState, type PropsWithChildren, type ReactNode } from 'react';
import { NavLink, useNavigate } from 'react-router-dom';

import { useAuth } from '@/features/auth/auth-context';
import { Button } from '@/shared/ui/button';
import { Card } from '@/shared/ui/card';
import { cn } from '@/shared/lib/cn';

interface ResumeShellProps {
  actions?: ReactNode;
  description: string;
  eyebrow: string;
  title: string;
}

export function ResumeShell({
  actions,
  children,
  description,
  eyebrow,
  title,
}: PropsWithChildren<ResumeShellProps>) {
  const navigate = useNavigate();
  const { logout, session } = useAuth();
  const [logoutOpen, setLogoutOpen] = useState(false);

  return (
    <main className="min-h-screen bg-transparent px-4 py-4 sm:px-6 lg:px-8">
      <div className="mx-auto flex max-w-7xl flex-col gap-6">
        <Card className="rounded-[2rem] border-slate-300/70 bg-white/92 backdrop-blur-sm dark:border-slate-700 dark:bg-slate-950/80">
          <div className="flex flex-col gap-5 lg:flex-row lg:items-start lg:justify-between">
            <div>
              <p className="text-xs font-semibold uppercase tracking-[0.3em] text-teal-800 dark:text-teal-300">
                {eyebrow}
              </p>
              <h1 className="mt-3 font-serif text-3xl font-semibold tracking-tight text-slate-950 dark:text-white sm:text-4xl">
                {title}
              </h1>
              <p className="mt-3 max-w-2xl text-sm leading-7 text-slate-700 dark:text-slate-300">
                {description}
              </p>
            </div>

            <div className="flex flex-col gap-3 sm:flex-row sm:items-center">
              <Card className="rounded-2xl border-slate-300/70 bg-slate-50/90 px-4 py-3 shadow-none dark:border-slate-800 dark:bg-slate-900/80">
                <p className="text-xs uppercase tracking-[0.24em] text-slate-600">
                  Session
                </p>
                <p className="mt-1 text-sm font-medium text-slate-900 dark:text-white">
                  {session?.displayName}
                </p>
                <p className="text-xs text-slate-600 dark:text-slate-400">
                  {session?.email}
                </p>
              </Card>
              {actions}
            </div>
          </div>

          <div className="mt-6 flex flex-wrap items-center gap-3">
            <WorkspaceTab
              icon={<RectangleStackIcon className="h-4 w-4" />}
              label="Dashboard"
              to="/app/resume/dashboard"
            />
            <WorkspaceTab
              icon={<PencilSquareIcon className="h-4 w-4" />}
              label="Builder"
              to="/app/resume/builder"
            />
            <WorkspaceTab
              icon={<PresentationChartBarIcon className="h-4 w-4" />}
              label="Preview"
              to="/app/resume/preview"
            />
            <Button
              className="ml-auto"
              onClick={() => setLogoutOpen(true)}
              variant="ghost"
            >
              <ArrowLeftStartOnRectangleIcon className="h-4 w-4" />
              Logout
            </Button>
          </div>
        </Card>

        {children}
      </div>

      <Dialog className="relative z-50" open={logoutOpen} onClose={setLogoutOpen}>
        <DialogBackdrop className="fixed inset-0 bg-slate-950/45 backdrop-blur-sm" />
        <div className="fixed inset-0 flex items-center justify-center p-4">
          <DialogPanel className="w-full max-w-md rounded-[2rem] border border-white/10 bg-white p-6 shadow-2xl dark:bg-slate-900">
            <DialogTitle className="text-xl font-semibold text-slate-950 dark:text-white">
              Log out of the resume workspace?
            </DialogTitle>
            <p className="mt-3 text-sm leading-6 text-slate-600 dark:text-slate-300">
              Your draft stays on this device. The local session will be cleared
              and protected routes will require sign-in again.
            </p>
            <div className="mt-6 flex justify-end gap-3">
              <Button onClick={() => setLogoutOpen(false)} variant="outline">
                Stay here
              </Button>
              <Button
                onClick={() => {
                  logout();
                  navigate('/login', { replace: true });
                }}
                variant="primary"
              >
                Log out
              </Button>
            </div>
          </DialogPanel>
        </div>
      </Dialog>
    </main>
  );
}

function WorkspaceTab({
  icon,
  label,
  to,
}: {
  icon: ReactNode;
  label: string;
  to: string;
}) {
  return (
    <NavLink
          className={({ isActive }) =>
        cn(
          'inline-flex items-center gap-2 rounded-2xl border px-4 py-2 text-sm font-medium transition',
          isActive
            ? 'border-teal-300 bg-teal-50 text-teal-950 dark:border-teal-400/30 dark:bg-teal-400/10 dark:text-teal-200'
            : 'border-slate-300 bg-white/85 text-slate-700 hover:border-teal-300 hover:text-slate-950 dark:border-slate-800 dark:bg-slate-950/80 dark:text-slate-300',
        )
      }
      to={to}
    >
      {icon}
      {label}
    </NavLink>
  );
}
