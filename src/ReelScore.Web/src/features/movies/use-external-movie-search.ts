import { useQuery } from "@tanstack/react-query";
import { searchExternalMovies } from "@/api/external-movies";

type UseExternalMovieSearchParameters = {
  query: string;
  page: number;
};

export function useExternalMovieSearch({
  query,
  page,
}: UseExternalMovieSearchParameters) {
  const normalizedQuery = query.trim();

  return useQuery({
    queryKey: ["external-movies", "search", normalizedQuery, page],
    queryFn: () =>
      searchExternalMovies({
        query: normalizedQuery,
        page,
      }),
    enabled: normalizedQuery.length >= 2,
  });
}