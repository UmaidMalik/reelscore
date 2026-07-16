using ReelScore.Api.DataTransferObjects;

namespace ReelScore.Api.Services;

public enum ImportMovieStatus
{
    Imported,
    AlreadyImported,
    ExternalMovieNotFound,
    InvalidMovieData
}

public sealed record ImportMovieResult(
    ImportMovieStatus Status,
    MovieResponse? Movie = null);
