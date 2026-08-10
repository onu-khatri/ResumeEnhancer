import { Link } from 'react-router-dom';

import { Button } from '@/shared/ui/button';
import { Card } from '@/shared/ui/card';

export function NotFoundPage() {
  return (
    <main className="flex min-h-screen items-center justify-center bg-slate-950 px-4 py-10 text-white">
      <Card className="max-w-lg rounded-[2rem] border-white/10 bg-white/5 p-10 text-center backdrop-blur">
        <p className="text-sm font-semibold uppercase tracking-[0.3em] text-lime-300">
          404
        </p>
        <h1 className="mt-4 text-3xl font-semibold">This page does not exist.</h1>
        <p className="mt-3 text-sm leading-6 text-slate-300">
          The resume workspace only exposes the login, builder, and preview
          flows right now.
        </p>
        <Button asChild className="mt-8">
          <Link to="/">Back to the app</Link>
        </Button>
      </Card>
    </main>
  );
}
