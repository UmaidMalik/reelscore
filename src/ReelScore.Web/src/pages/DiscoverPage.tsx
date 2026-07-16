import { useState } from "react";
import type { FormEvent } from "react";
import { Search } from "lucide-react";
import { ExternalMovieCard } from "@/components/movies/ExternalMovieCard";
import { MovieCardSkeleton } from "@/components/movies/MovieCardSkeleton";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { useExternalMovieSearch } from "@/features/movies/use-external-movie-search";

export function DiscoverPage() {
  const [searchInput, setSearchInput] = useState("");
  const [query, setQuery] = useState("");
  const [page, setPage] = useState(1);

  const movieSearch = useExternalMovieSearch({
    query,
    page,
  });

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const normalizedQuery = searchInput.trim();

    if (normalizedQuery.length < 2) {
      return;
    }

    setPage(1);
    setQuery(normalizedQuery);
  }

  return (
    <section className="space-y-8">
      <div>
        <p className="text-sm font-medium text-orange-500">
          TMDB discovery
        </p>

        <h1 className="mt-2 text-3xl font-bold tracking-tight sm:text-4xl">
          Discover movies
        </h1>

        <p className="mt-2 max-w-2xl text-muted-foreground">
          Search TMDB and import movies into the ReelScore community
          library.
        </p>
      </div>

      <form
        onSubmit={handleSubmit}
        className="flex max-w-2xl flex-col gap-3 sm:flex-row"
      >
        <div className="relative flex-1">
          <Search className="absolute top-1/2 left-3 size-4 -translate-y-1/2 text-muted-foreground" />

          <Input
            value={searchInput}
            onChange={(event) => setSearchInput(event.target.value)}
            placeholder="Search for Alien, Arrival, Dune..."
            className="pl-9"
          />
        </div>

        <Button
          type="submit"
          className="bg-orange-500 text-white hover:bg-orange-600"
          disabled={searchInput.trim().length < 2}
        >
          Search
        </Button>
      </form>

      {!query && (
        <div className="rounded-xl border border-dashed p-10 text-center">
          <Search className="mx-auto size-10 text-muted-foreground" />

          <h2 className="mt-4 text-lg font-semibold">
            Search the movie catalogue
          </h2>

          <p className="mt-2 text-sm text-muted-foreground">
            Enter at least two characters to begin.
          </p>
        </div>
      )}

      {movieSearch.isError && (
        <Alert variant="destructive">
          <AlertTitle>Movie search failed</AlertTitle>
          <AlertDescription>
            {movieSearch.error instanceof Error
              ? movieSearch.error.message
              : "The movie search could not be completed."}
          </AlertDescription>
        </Alert>
      )}

      {movieSearch.isPending && query && (
        <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5">
          {Array.from({ length: 10 }).map((_, index) => (
            <MovieCardSkeleton key={index} />
          ))}
        </div>
      )}

      {movieSearch.data && (
        <div className="space-y-5">
          <div className="flex items-end justify-between gap-4">
            <div>
              <h2 className="text-xl font-semibold">
                Results for “{query}”
              </h2>

              <p className="text-sm text-muted-foreground">
                {movieSearch.data.totalResults.toLocaleString()} movies
                found
              </p>
            </div>

            <p className="text-sm text-muted-foreground">
              Page {movieSearch.data.page} of{" "}
              {movieSearch.data.totalPages}
            </p>
          </div>

          {movieSearch.data.results.length === 0 ? (
            <div className="rounded-xl border border-dashed p-10 text-center">
              <h2 className="font-semibold">No movies found</h2>
              <p className="mt-2 text-sm text-muted-foreground">
                Try a different movie title.
              </p>
            </div>
          ) : (
            <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5">
              {movieSearch.data.results.map((movie) => (
                <ExternalMovieCard
                  key={movie.tmdbId}
                  movie={movie}
                />
              ))}
            </div>
          )}

          {movieSearch.data.totalPages > 1 && (
            <div className="flex justify-center gap-3 pt-4">
              <Button
                variant="outline"
                disabled={page <= 1 || movieSearch.isFetching}
                onClick={() =>
                  setPage((currentPage) =>
                    Math.max(1, currentPage - 1),
                  )
                }
              >
                Previous
              </Button>

              <Button
                variant="outline"
                disabled={
                  page >= movieSearch.data.totalPages ||
                  movieSearch.isFetching
                }
                onClick={() =>
                  setPage((currentPage) => currentPage + 1)
                }
              >
                Next
              </Button>
            </div>
          )}
        </div>
      )}
    </section>
  );
}