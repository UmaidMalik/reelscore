import { useQuery } from "@tanstack/react-query";
import { getExternalTitleDetails } from "@/api/external-movies";
import type { MediaType } from "@/types/movies";

type UseExternalTitleDetailsParameters = {
  mediaType: MediaType;
  tmdbId: number;
};

export function useExternalTitleDetails({
  mediaType,
  tmdbId,
}: UseExternalTitleDetailsParameters) {
  return useQuery({
    queryKey: ["external-title", mediaType, tmdbId],
    queryFn: () =>
      getExternalTitleDetails(mediaType, tmdbId),
    enabled: Number.isInteger(tmdbId) && tmdbId > 0,
  });
}