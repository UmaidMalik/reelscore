import { apiRequest } from "@/api/client";
import type { LocalTitle } from "@/types/movies";

export function getLocalTitles(): Promise<LocalTitle[]> {
  return apiRequest<LocalTitle[]>("/api/movies");
}