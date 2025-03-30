namespace BookTesterApp.Core.Models;

public class Book
{
    public int Index { get; set; }

    public string ISBN { get; set; }

    public string Title { get; set; }

    public List<string> Authors { get; set; }

    public string Publisher { get; set; }

    public string CoverUrl { get; set; }

    public int Likes { get; set; }

    public List<Review> Reviews { get; set; } = new();
}