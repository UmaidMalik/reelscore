import {
  ArrowLeft,
  CalendarDays,
  Clock,
  Film,
  Layers3,
  ListVideo,
  Star,
} from "lucide-react";
import { Link, useParams } from "react-router";
import { toast } from "sonner";
import { ApiError } from "@/api/client";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { useExternalTitleDetails } from "@/features/movies/use-external-title-details";
import { useImportTitle } from "@/features/movies/use-import-title";
import type { MediaType } from "@/types/movies";

function isMediaType(value: string | undefined): value is MediaType {
  return value === "movie" || value === "tv";
}

export function ExternalTitleDetailsPage() {
  const parameters = useParams<{
    mediaType: string;
    tmdbId: string;
  }>();

  const mediaType = isMediaType(parameters.mediaType)
    ? parameters.mediaType
    : null;

  const tmdbId = Number(parameters.tmdbId);

  const detailsQuery = useExternalTitleDetails({
    mediaType: mediaType ?? "movie",
    tmdbId,
  });

  const importMutation = useImportTitle();

  if (!mediaType || !Number.isInteger(tmdbId) || tmdbId <= 0) {
    return (
      <Alert variant="destructive">
        <AlertTitle>Invalid title</AlertTitle>
        <AlertDescription>
          The requested media type or TMDB ID is invalid.
        </AlertDescription>
      </Alert>
    );
  }

  const validMediaType: MediaType = mediaType;

  function handleImport() {
    importMutation.mutate(
      {
        mediaType: validMediaType,
        tmdbId,
      },
      {
        onSuccess: (title) => {
          toast.success(`${title.title} was added to ReelScore.`);
        },
        onError: (error) => {
          if (error instanceof ApiError && error.status === 409) {
            toast.info("This title is already in the ReelScore library.");
            return;
          }

          toast.error(
            error instanceof Error
              ? error.message
              : "The title could not be imported.",
          );
        },
      },
    );
  }

  if (detailsQuery.isPending) {
    return (
      <section className="space-y-6">
        <Skeleton className="h-6 w-32" />
        <Skeleton className="aspect-[16/7] w-full rounded-xl" />
        <div className="grid gap-6 md:grid-cols-[220px_1fr]">
          <Skeleton className="aspect-[2/3] w-full rounded-xl" />
          <div className="space-y-4">
            <Skeleton className="h-10 w-2/3" />
            <Skeleton className="h-5 w-1/3" />
            <Skeleton className="h-24 w-full" />
          </div>
        </div>
      </section>
    );
  }

  if (detailsQuery.isError) {
    return (
      <Alert variant="destructive">
        <AlertTitle>Unable to load details</AlertTitle>
        <AlertDescription>
          {detailsQuery.error instanceof Error
            ? detailsQuery.error.message
            : "The title details could not be loaded."}
        </AlertDescription>
      </Alert>
    );
  }

  const title = detailsQuery.data;

  return (
    <section className="space-y-6">
      <Link
        to="/discover"
        className="inline-flex items-center gap-2 text-sm text-muted-foreground hover:text-foreground"
      >
        <ArrowLeft className="size-4" />
        Back to Discover
      </Link>

      <div className="relative overflow-hidden rounded-2xl border bg-card">
        {title.backdropUrl ? (
          <img
            src={title.backdropUrl}
            alt=""
            className="aspect-[16/7] w-full object-cover opacity-45"
          />
        ) : (
          <div className="aspect-[16/7] bg-muted" />
        )}

        <div className="absolute inset-0 bg-gradient-to-t from-background via-background/40 to-transparent" />
      </div>

      <div className="-mt-28 grid gap-6 px-4 md:grid-cols-[220px_1fr] md:px-8">
        <div className="relative overflow-hidden rounded-xl border bg-muted shadow-xl">
          {title.posterUrl ? (
            <img
              src={title.posterUrl}
              alt={`${title.title} poster`}
              className="aspect-[2/3] w-full object-cover"
            />
          ) : (
            <div className="flex aspect-[2/3] items-center justify-center">
              <Film className="size-12 text-muted-foreground" />
            </div>
          )}
        </div>

        <div className="relative space-y-5 pt-16 md:pt-24">
          <div className="flex flex-wrap items-center gap-2">
            <Badge>
              {title.mediaType === "movie" ? "Movie" : "TV Series"}
            </Badge>

            {title.status && (
              <Badge variant="secondary">{title.status}</Badge>
            )}
          </div>

          <div>
            <h1 className="text-3xl font-bold tracking-tight sm:text-5xl">
              {title.title}
            </h1>

            {title.originalTitle !== title.title && (
              <p className="mt-2 text-muted-foreground">
                {title.originalTitle}
              </p>
            )}
          </div>

          <div className="flex flex-wrap gap-x-5 gap-y-2 text-sm text-muted-foreground">
            <span className="flex items-center gap-1.5">
              <Star className="size-4 fill-orange-400 text-orange-400" />
              {title.tmdbScore.toFixed(1)}
            </span>

            <span className="flex items-center gap-1.5">
              <CalendarDays className="size-4" />
              {title.releaseYear ?? "Unknown year"}
            </span>

            {title.runtimeMinutes && (
              <span className="flex items-center gap-1.5">
                <Clock className="size-4" />
                {title.runtimeMinutes} min
              </span>
            )}

            {title.numberOfSeasons !== null && (
              <span className="flex items-center gap-1.5">
                <Layers3 className="size-4" />
                {title.numberOfSeasons} seasons
              </span>
            )}

            {title.numberOfEpisodes !== null && (
              <span className="flex items-center gap-1.5">
                <ListVideo className="size-4" />
                {title.numberOfEpisodes} episodes
              </span>
            )}
          </div>

          <div className="flex flex-wrap gap-2">
            {title.genres.map((genre) => (
              <Badge key={genre} variant="outline">
                {genre}
              </Badge>
            ))}
          </div>

          <p className="max-w-3xl leading-7 text-muted-foreground">
            {title.overview || "No overview is available."}
          </p>

          <Button
            type="button"
            onClick={handleImport}
            disabled={importMutation.isPending}
            className="bg-orange-500 text-white hover:bg-orange-600"
          >
            {importMutation.isPending
              ? "Adding to ReelScore..."
              : "Add to ReelScore"}
          </Button>
        </div>
      </div>
    </section>
  );
}