export function LibraryPage() {
  return (
    <section className="space-y-3">
      <p className="text-sm font-medium text-orange-500">
        ReelScore library
      </p>

      <h1 className="text-3xl font-bold tracking-tight sm:text-4xl">
        Community Library
      </h1>

      <p className="max-w-2xl text-muted-foreground">
        Movies imported from TMDB into the local ReelScore database will
        appear here.
      </p>

      <div className="rounded-xl border border-dashed p-10 text-center">
        <h2 className="font-semibold">No library view yet</h2>

        <p className="mt-2 text-sm text-muted-foreground">
          We’ll connect this page to the local movies endpoint next.
        </p>
      </div>
    </section>
  );
}