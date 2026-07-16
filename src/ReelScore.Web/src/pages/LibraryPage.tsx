import { useMemo, useState } from "react";
import {
  ArrowDownAZ,
  Film,
  Search,
} from "lucide-react";
import { Link } from "react-router";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { LocalTitleCard } from "@/components/movies/LocalTitleCard";
import { MovieCardSkeleton } from "@/components/movies/MovieCardSkeleton";
import { useLocalTitles } from "@/features/movies/use-local-titles";
import type { LocalTitle } from "@/types/movies";

type MediaFilter = "all" | "movie" | "tv";
type SortOption = "title" | "year" | "rating";

function matchesMediaFilter(
  title: LocalTitle,
  filter: MediaFilter,
) {
  if (filter === "all") {
    return true;
  }

  if (filter === "movie") {
    return title.mediaType === "movie";
  }

  return title.mediaType === "tv";
}

export function LibraryPage() {
  const titlesQuery = useLocalTitles();

  const [searchInput, setSearchInput] = useState("");
  const [mediaFilter, setMediaFilter] =
    useState<MediaFilter>("all");

  const [sortOption, setSortOption] =
    useState<SortOption>("title");

  const filteredTitles = useMemo(() => {
    const normalizedSearch = searchInput
      .trim()
      .toLowerCase();

    return [...(titlesQuery.data ?? [])]
      .filter((title) =>
        matchesMediaFilter(title, mediaFilter),
      )
      .filter((title) => {
        if (!normalizedSearch) {
          return true;
        }

        return (
          title.title
            .toLowerCase()
            .includes(normalizedSearch) ||
          title.originalTitle
            ?.toLowerCase()
            .includes(normalizedSearch)
        );
      })
      .sort((first, second) => {
        if (sortOption === "year") {
          return (
            (second.releaseYear ?? 0) -
            (first.releaseYear ?? 0)
          );
        }

        if (sortOption === "rating") {
          return (
            second.averageRating -
            first.averageRating
          );
        }

        return first.title.localeCompare(second.title);
      });
  }, [
    mediaFilter,
    searchInput,
    sortOption,
    titlesQuery.data,
  ]);

  if (titlesQuery.isPending) {
    return (
      <section className="space-y-8">
        <LibraryHeader />

        <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5">
          {Array.from({ length: 10 }).map((_, index) => (
            <MovieCardSkeleton key={index} />
          ))}
        </div>
      </section>
    );
  }

  if (titlesQuery.isError) {
    return (
      <Alert variant="destructive">
        <AlertTitle>Unable to load the library</AlertTitle>
        <AlertDescription>
          {titlesQuery.error instanceof Error
            ? titlesQuery.error.message
            : "The Community Library could not be loaded."}
        </AlertDescription>
      </Alert>
    );
  }

  const hasTitles = titlesQuery.data.length > 0;

  return (
    <section className="space-y-8">
      <LibraryHeader />

      {!hasTitles ? (
        <div className="rounded-xl border border-dashed p-12 text-center">
          <Film className="mx-auto size-12 text-muted-foreground" />

          <h2 className="mt-4 text-xl font-semibold">
            The Community Library is empty
          </h2>

          <p className="mx-auto mt-2 max-w-md text-sm text-muted-foreground">
            Discover a movie or TV series and add it to
            ReelScore.
          </p>

          <Link
            to="/discover"
            className="mt-6 inline-flex h-9 items-center justify-center rounded-md bg-orange-500 px-4 text-sm font-medium text-white transition-colors hover:bg-orange-600"
          >
            Discover titles
          </Link>
        </div>
      ) : (
        <>
          <div className="flex flex-col gap-4 rounded-xl border bg-card p-4 lg:flex-row lg:items-center">
            <div className="relative flex-1">
              <Search className="absolute top-1/2 left-3 size-4 -translate-y-1/2 text-muted-foreground" />

              <Input
                value={searchInput}
                onChange={(event) =>
                  setSearchInput(event.target.value)
                }
                placeholder="Search the ReelScore library..."
                className="pl-9"
              />
            </div>

            <div className="flex flex-wrap gap-2">
              <Button
                type="button"
                variant={
                  mediaFilter === "all"
                    ? "default"
                    : "outline"
                }
                onClick={() => setMediaFilter("all")}
              >
                All
              </Button>

              <Button
                type="button"
                variant={
                  mediaFilter === "movie"
                    ? "default"
                    : "outline"
                }
                onClick={() => setMediaFilter("movie")}
              >
                Movies
              </Button>

              <Button
                type="button"
                variant={
                  mediaFilter === "tv"
                    ? "default"
                    : "outline"
                }
                onClick={() => setMediaFilter("tv")}
              >
                TV Series
              </Button>
            </div>

            <label className="flex items-center gap-2 text-sm">
              <ArrowDownAZ className="size-4 text-muted-foreground" />

              <select
                value={sortOption}
                onChange={(event) =>
                  setSortOption(
                    event.target.value as SortOption,
                  )
                }
                className="h-9 rounded-md border border-input bg-background px-3 text-sm"
              >
                <option value="title">
                  Title
                </option>
                <option value="year">
                  Release year
                </option>
                <option value="rating">
                  ReelScore rating
                </option>
              </select>
            </label>
          </div>

          <div className="flex items-center justify-between gap-4">
            <p className="text-sm text-muted-foreground">
              {filteredTitles.length.toLocaleString()}{" "}
              {filteredTitles.length === 1
                ? "title"
                : "titles"}
            </p>
          </div>

          {filteredTitles.length === 0 ? (
            <div className="rounded-xl border border-dashed p-10 text-center">
              <h2 className="font-semibold">
                No matching titles
              </h2>

              <p className="mt-2 text-sm text-muted-foreground">
                Change the search text or media filter.
              </p>
            </div>
          ) : (
            <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5">
              {filteredTitles.map((title) => (
                <LocalTitleCard
                  key={title.movieId}
                  title={title}
                />
              ))}
            </div>
          )}
        </>
      )}
    </section>
  );
}

function LibraryHeader() {
  return (
    <div>
      <p className="text-sm font-medium text-orange-500">
        ReelScore catalogue
      </p>

      <h1 className="mt-2 text-3xl font-bold tracking-tight sm:text-4xl">
        Community Library
      </h1>

      <p className="mt-2 max-w-2xl text-muted-foreground">
        Browse movies and television series available
        for community ratings, reviews, favourites, and
        watchlists.
      </p>
    </div>
  );
}