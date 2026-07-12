namespace ReelScore.Api.DataTransferObjects;


public class MovieDto
{
    public long MovieId
    {
        get; set;
    }

    public string Title { get; set; } = string.Empty;

    public string? Summary
    {
        get; set;
    }

    public int ReleaseYear
    {
        get; set;
    }
}
