export type MediaType = "movie" | "tv";

export type ExternalMovieSearchItem = {
  tmdbId: number;
  title: string;
  originalTitle: string;
  overview: string;
  releaseDate: string | null;
  releaseYear: number | null;
  posterUrl: string | null;
  backdropUrl: string | null;
  genreIds: number[];
  originalLanguage: string;
  popularity: number;
  tmdbScore: number;
  tmdbVoteCount: number;
  mediaType: MediaType;
};

export type ExternalMovieSearchResponse = {
  page: number;
  totalPages: number;
  totalResults: number;
  results: ExternalMovieSearchItem[];
};
