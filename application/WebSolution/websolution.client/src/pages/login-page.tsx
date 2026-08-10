import { ArrowRightIcon, ShieldCheckIcon } from '@heroicons/react/24/outline';
import { Navigate, useLocation } from 'react-router-dom';

import { useAuth } from '@/features/auth/auth-context';
import { Button } from '@/shared/ui/button';
import { Card } from '@/shared/ui/card';

export function LoginPage() {
  const location = useLocation();
  const { isAuthenticated, signIn } = useAuth();

  if (isAuthenticated) {
    return <Navigate replace to="/app/resume/dashboard" />;
  }

  const redirectTo =
    (location.state as { from?: { pathname?: string } } | null)?.from
      ?.pathname ?? '/app/resume/dashboard';

  return (
    <main className="min-h-screen px-4 py-8 text-slate-900 sm:px-6 lg:px-8">
      <div className="mx-auto flex min-h-[calc(100vh-4rem)] max-w-6xl items-center">
        <div className="grid w-full gap-8 lg:grid-cols-[1.15fr_0.85fr]">
          <section className="rounded-[2rem] border border-slate-300/70 bg-white/92 p-8 shadow-[0_28px_100px_rgba(15,23,42,0.12)] backdrop-blur-sm dark:border-slate-700 dark:bg-slate-900/78">
            <p className="text-sm font-semibold uppercase tracking-[0.3em] text-teal-800 dark:text-teal-300">
              Resume Enhancer
            </p>
            <h1 className="mt-6 max-w-xl font-serif text-4xl font-semibold tracking-tight text-slate-950 dark:text-white sm:text-5xl">
              Build a resume workspace that feels calm, credible, and ready to send.
            </h1>
            <p className="mt-5 max-w-2xl text-base leading-8 text-slate-700 dark:text-slate-300">
              This frontend is wired for protected routes, session refresh, and
              cookie-based API usage. For now, we are using a local workspace
              session so the resume flow can move ahead before backend auth is
              connected.
            </p>

            <div className="mt-8 grid gap-4 sm:grid-cols-3">
              {[
                'Draft-safe multi-step editing',
                'Typed API integration with refresh hooks',
                'Accessible, mobile-first recruiter preview',
              ].map((item) => (
                <Card key={item} className="border-slate-300/70 bg-slate-50/85">
                  <p className="text-sm font-medium text-slate-800">{item}</p>
                </Card>
              ))}
            </div>
          </section>

          <Card className="rounded-[2rem] border-slate-300/70 bg-white p-8 shadow-[0_30px_110px_rgba(15,23,42,0.14)] dark:border-slate-700 dark:bg-slate-900">
            <div className="inline-flex h-12 w-12 items-center justify-center rounded-2xl bg-teal-100 text-teal-800 dark:bg-teal-500/10 dark:text-teal-300">
              <ShieldCheckIcon className="h-6 w-6" />
            </div>
            <h2 className="mt-6 text-2xl font-semibold text-slate-950 dark:text-white">
              Continue into the workspace
            </h2>
            <p className="mt-3 text-sm leading-7 text-slate-700 dark:text-slate-300">
              Use the temporary local session now. When the real authentication
              endpoints are ready, the provider and API client already have the
              seams to swap in secure HTTP-only cookie flows.
            </p>

            <Button
              className="mt-8 w-full"
              onClick={() => {
                signIn();
                window.location.assign(redirectTo);
              }}
            >
              Continue as workspace user
              <ArrowRightIcon className="h-4 w-4" />
            </Button>

            <p className="mt-4 text-xs leading-5 text-slate-600 dark:text-slate-400">
              Session state is persisted locally and mirrored with a lightweight
              cookie marker so the API layer is already configured for
              `credentials: include`.
            </p>
          </Card>
        </div>
      </div>
    </main>
  );
}
