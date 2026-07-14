import { CalendarDays, Star } from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardFooter } from "@/components/ui/card";
import { MoviePoster } from "@/components/movies/MoviePoster";
import type { ExternalMovieSearchItem } from "@/types/movies";

type ExternalMovieCardProps = {
  movie: ExternalMovieSearchItem;
};

export function ExternalMovieCard({
  movie,
}: ExternalMovieCardProps) {
  return (
    <Card className="group overflow-hidden border-border/70 bg-card/80 py-0 transition hover:-translate-y-1 hover:border-orange-500/50">
      <div className="relative overflow-hidden">
        <MoviePoster
          src={movie.posterUrl}
          alt={`${movie.title} poster`}
        />

        <Badge className="absolute top-3 right-3 bg-black/75 text-white">
          <Star className="size-3 fill-orange-400 text-orange-400" />
          {movie.tmdbScore.toFixed(1)}
        </Badge>
      </div>

      <CardContent className="space-y-2 p-4">
        <div>
          <h2 className="line-clamp-1 font-semibold">
            {movie.title}
          </h2>

          {movie.originalTitle !== movie.title && (
            <p className="line-clamp-1 text-xs text-muted-foreground">
              {movie.originalTitle}
            </p>
          )}
        </div>

        <div className="flex items-center gap-1 text-sm text-muted-foreground">
          <CalendarDays className="size-4" />
          <span>{movie.releaseYear ?? "Unknown year"}</span>
        </div>

        <p className="line-clamp-3 text-sm text-muted-foreground">
          {movie.overview || "No overview is available."}
        </p>
      </CardContent>

      <CardFooter className="mt-auto p-4 pt-0">
        <Button
          className="w-full bg-orange-500 text-white hover:bg-orange-600"
          disabled
        >
          View details
        </Button>
      </CardFooter>
    </Card>
  );
}