namespace ReelScore.Api.DataTransferObjects;

public class MovieDto
{
    public long MovieId
    {
        get; set;
    }
    public string Title { get; set; } = null!;
    public string Summary { get; set; } = null!;
    public int ReleaseYear
    {
        get; set;
    }
}
