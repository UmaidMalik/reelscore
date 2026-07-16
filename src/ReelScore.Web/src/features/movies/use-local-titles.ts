import { useQuery } from "@tanstack/react-query";
import { getLocalTitles } from "@/api/local-titles";
export const localTitlesQueryKey = ["local-titles"] as const;

export function useLocalTitles() {
  return useQuery({
    queryKey: localTitlesQueryKey,
    queryFn: getLocalTitles,
  });
}