export type MediaType = "movie" | "tv";

export type ExternalMovieSearchItem = {
  tmdbId: number;
  mediaType: MediaType;
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
};

export type ExternalMovieSearchResponse = {
  page: number;
  totalPages: number;
  totalResults: number;
  results: ExternalMovieSearchItem[];
};

export type ExternalTitleDetails = {
  tmdbId: number;
  mediaType: MediaType;
  title: string;
  originalTitle: string;
  overview: string;
  releaseDate: string | null;
  releaseYear: number | null;
  runtimeMinutes: number | null;
  numberOfSeasons: number | null;
  numberOfEpisodes: number | null;
  posterPath: string | null;
  posterUrl: string | null;
  backdropPath: string | null;
  backdropUrl: string | null;
  genres: string[];
  originalLanguage: string;
  tmdbScore: number;
  tmdbVoteCount: number;
  status: string | null;
};

export type LocalTitle = {
  movieId: number;
  tmdbId: number | null;
  mediaType: MediaType;
  title: string;
  originalTitle: string | null;
  summary: string | null;
  releaseDate: string | null;
  releaseYear: number | null;
  runtimeMinutes: number | null;
  numberOfSeasons: number | null;
  numberOfEpisodes: number | null;
  posterUrl: string | null;
  backdropUrl: string | null;
  genres: string[];
  averageRating: number;
  ratingCount: number;
};