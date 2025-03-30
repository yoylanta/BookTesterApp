using BookTesterApp.Core.Models;
using BookTesterApp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookTesterApp.Presentation.Controllers;

[Route("Books")]
public class BooksController : Controller
{
    private const int FirstLoadCount = 20;
    private const int AdditionalLoadCount = 10;

    [HttpGet]
    [IgnoreAntiforgeryToken]
    [Route("Index")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [IgnoreAntiforgeryToken]
    [Route("LoadBooks")]
    public IActionResult LoadBooks(
        string language = "en",
        int seed = 12345,
        double avgLikes = 5.0,
        double avgReviews = 4.7,
        int page = 1)
    {
        int count = (page == 1) ? FirstLoadCount : AdditionalLoadCount;

        int combinedSeed = seed + page;

        int startIndex = 1 + (page == 1 ? 0 : FirstLoadCount + (page - 2) * AdditionalLoadCount);

        List<Book> books = BookService.GenerateBooks(
            count,
            language,
            combinedSeed,
            avgLikes,
            avgReviews,
            startIndex
        );

        return Json(books);
    }
}