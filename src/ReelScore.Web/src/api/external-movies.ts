import { apiRequest } from "@/api/client";
import type {
  ExternalMovieSearchResponse,
  ExternalTitleDetails,
  LocalTitle,
  MediaType,
} from "@/types/movies";

type SearchExternalMoviesParameters = {
  query: string;
  page?: number;
  language?: string;
  releaseYear?: number;
};

export async function searchExternalMovies({
  query,
  page = 1,
  language = "en-US",
  releaseYear,
}: SearchExternalMoviesParameters): Promise<ExternalMovieSearchResponse> {
  const searchParameters = new URLSearchParams({
    query,
    page: page.toString(),
    language,
  });

  if (releaseYear !== undefined) {
    searchParameters.set("releaseYear", releaseYear.toString());
  }

  return apiRequest<ExternalMovieSearchResponse>(
    `/api/external/movies/search?${searchParameters.toString()}`,
  );
}

export function getExternalTitleDetails(
  mediaType: MediaType,
  tmdbId: number,
  language = "en-US",
): Promise<ExternalTitleDetails> {
  return apiRequest<ExternalTitleDetails>(
    `/api/external/movies/${mediaType}/${tmdbId}?language=${encodeURIComponent(language)}`,
  );
}

export function importExternalTitle(
  mediaType: MediaType,
  tmdbId: number,
  language = "en-US",
): Promise<LocalTitle> {
  return apiRequest<LocalTitle>(
    `/api/movies/import/${mediaType}/${tmdbId}?language=${encodeURIComponent(language)}`,
    {
      method: "POST",
    },
  );
}