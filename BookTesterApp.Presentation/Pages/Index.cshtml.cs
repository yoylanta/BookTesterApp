using BookTesterApp.Core.Models;
using BookTesterApp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookTesterApp.Presentation.Pages;

public class IndexModel : PageModel
{
    private const int InitialBatchSize = 20;
    private const int AdditionalBatchSize = 10;

    public List<Book> Books { get; private set; } = new();

    [BindProperty(SupportsGet = true)]
    public string Language { get; set; } = "en";

    [BindProperty(SupportsGet = true)]
    public int Seed { get; set; } = 12345;

    [BindProperty(SupportsGet = true)]
    public double AvgLikes { get; set; } = 5.0;

    [BindProperty(SupportsGet = true)]
    public double AvgReviews { get; set; } = 4.7;

    [BindProperty(SupportsGet = true)]
    public int Page { get; set; } = 1;

    public void OnGet()
    {
        LoadBooks(InitialBatchSize);
    }

    public IActionResult OnGetLoadMore(int page)
    {
        int combinedSeed = Seed + page;
        var newBooks = BookService.GenerateBooks(AdditionalBatchSize, Language, combinedSeed, AvgLikes, AvgReviews);
        return new JsonResult(newBooks);
    }

    private void LoadBooks(int count)
    {
        int combinedSeed = Seed + Page;
        Books = BookService.GenerateBooks(count, Language, combinedSeed, AvgLikes, AvgReviews);
    }
}