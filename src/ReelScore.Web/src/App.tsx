import { Navigate, Route, Routes } from "react-router";
import { AppLayout } from "@/components/layout/AppLayout";
import { DiscoverPage } from "@/pages/DiscoverPage";
import { ExternalTitleDetailsPage } from "@/pages/ExternalTitleDetailsPage";
import { LibraryPage } from "@/pages/LibraryPage";
import { NotFoundPage } from "@/pages/NotFoundPage";

export default function App() {
  return (
    <Routes>
      <Route element={<AppLayout />}>
        <Route index element={<Navigate replace to="/discover" />} />
        <Route path="/discover" element={<DiscoverPage />} />

        <Route
          path="/external/:mediaType/:tmdbId"
          element={<ExternalTitleDetailsPage />}
        />

        <Route path="/library" element={<LibraryPage />} />
        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  );
}