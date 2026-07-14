import { Film } from "lucide-react";

type MoviePosterProps = {
  src: string | null;
  alt: string;
};

export function MoviePoster({ src, alt }: MoviePosterProps) {
  if (!src) {
    return (
      <div className="flex aspect-[2/3] items-center justify-center bg-muted">
        <Film className="size-10 text-muted-foreground" />
      </div>
    );
  }

  return (
    <img
      src={src}
      alt={alt}
      loading="lazy"
      className="aspect-[2/3] w-full object-cover"
    />
  );
}