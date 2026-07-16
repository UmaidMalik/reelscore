import { Link } from "react-router";

export function NotFoundPage() {
  return (
    <section className="flex min-h-[60vh] flex-col items-center justify-center text-center">
      <p className="text-sm font-medium text-orange-500">404</p>

      <h1 className="mt-2 text-3xl font-bold tracking-tight">
        Page not found
      </h1>

      <p className="mt-3 max-w-md text-muted-foreground">
        The page you tried to open does not exist.
      </p>

        <Link
        to="/discover"
        className="mt-6 inline-flex h-9 items-center justify-center rounded-md bg-primary px-4 text-sm font-medium text-primary-foreground shadow-xs transition-colors hover:bg-primary/90"
        >
        Return to Discover
        </Link>
    </section>
  );
}