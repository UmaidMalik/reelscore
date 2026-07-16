import { Film, Star, Tv } from "lucide-react";
import { Link } from "react-router";

import { Badge } from "@/components/ui/badge";
import type { LocalTitle } from "@/types/movies";

type LocalTitleCardProps = {
  title: LocalTitle;
};

export function LocalTitleCard({ title }: LocalTitleCardProps) {
  const isMovie = title.mediaType === "movie";

  const metadata = isMovie
    ? title.runtimeMinutes
      ? `${title.runtimeMinutes} min`
      : null
    : title.numberOfSeasons
      ? `${title.numberOfSeasons} ${
          title.numberOfSeasons === 1 ? "season" : "seasons"
        }`
      : null;

  return (
    <Link
      to={`/movies/${title.movieId}`}
      className="group overflow-hidden rounded-xl border bg-card transition hover:-translate-y-1 hover:shadow-lg focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
    >
      <div className="aspect-[2/3] overflow-hidden bg-muted">
        {title.posterUrl ? (
          <img
            src={title.posterUrl}
            alt={`${title.title} poster`}
            className="h-full w-full object-cover transition duration-300 group-hover:scale-105"
            loading="lazy"
          />
        ) : (
          <div className="flex h-full items-center justify-center text-muted-foreground">
            {isMovie ? (
              <Film className="size-12" aria-hidden="true" />
            ) : (
              <Tv className="size-12" aria-hidden="true" />
            )}
            <span className="sr-only">Poster unavailable</span>
          </div>
        )}
      </div>

      <div className="space-y-3 p-4">
        <div className="space-y-1">
          <div className="flex items-start justify-between gap-2">
            <h2 className="line-clamp-2 font-semibold leading-tight">
              {title.title}
            </h2>

            <Badge variant="secondary" className="shrink-0">
              {isMovie ? "Movie" : "TV"}
            </Badge>
          </div>

          <p className="text-sm text-muted-foreground">
            {[title.releaseYear, metadata].filter(Boolean).join(" · ") ||
              "Release details unavailable"}
          </p>
        </div>

        {title.genres.length > 0 && (
          <p className="line-clamp-1 text-sm text-muted-foreground">
            {title.genres.slice(0, 3).join(", ")}
          </p>
        )}

        <div className="flex items-center gap-1 text-sm">
          <Star
            className="size-4 fill-current text-amber-500"
            aria-hidden="true"
          />

          {title.ratingCount > 0 ? (
            <>
              <span className="font-medium">
                {title.averageRating.toFixed(1)}
              </span>
              <span className="text-muted-foreground">
                ({title.ratingCount})
              </span>
            </>
          ) : (
            <span className="text-muted-foreground">Not yet rated</span>
          )}
        </div>
      </div>
    </Link>
  );
}