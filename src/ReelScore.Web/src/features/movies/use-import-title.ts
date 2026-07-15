import { useMutation, useQueryClient } from "@tanstack/react-query";
import { importExternalTitle } from "@/api/external-movies";
import type { MediaType } from "@/types/movies";

type ImportTitleParameters = {
  mediaType: MediaType;
  tmdbId: number;
};

export function useImportTitle() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      mediaType,
      tmdbId,
    }: ImportTitleParameters) =>
      importExternalTitle(mediaType, tmdbId),

    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["local-titles"],
      });
    },
  });
}