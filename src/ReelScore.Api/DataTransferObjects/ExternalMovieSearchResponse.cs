namespace ReelScore.Api.DataTransferObjects;

public sealed class ExternalMovieSearchResponse
{
    public int Page
    {
        get; init;
    }

    public int TotalPages
    {
        get; init;
    }

    public int TotalResults
    {
        get; init;
    }

    public IReadOnlyCollection<ExternalMovieSearchItemResponse> Results
    {
        get;
        init;
    } = Array.Empty<ExternalMovieSearchItemResponse>();
}
