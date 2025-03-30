using Bogus;
using BookTesterApp.Core.Models;

namespace BookTesterApp.Infrastructure.Services;

public class BookService
{
    private static readonly string[] SupportedLangs = { "en", "de", "fr" };

    private static readonly Dictionary<string, string[]> TitlePrefixes = new()
    {
        { "en", new[] { "Shadows of", "Secrets of", "Mystery of", "Chronicles of", "Tales of" } },
        { "de", new[] { "Schatten von", "Geheimnisse von", "Mysterium von", "Chroniken von", "Erzählungen von" } },
        { "fr", new[] { "Ombres de", "Secrets de", "Mystère de", "Chroniques de", "Contes de" } }
    };

    private static readonly Dictionary<string, string[]> TitleSuffixes = new()
    {
        { "en", new[] { "the Silent Forest", "the Lost Kingdom", "the Forgotten City", "the Hidden Realm" } },
        { "de", new[] { "dem stillen Wald", "dem verlorenen Königreich", "der vergessenen Stadt", "dem verborgenen Reich" } },
        { "fr", new[] { "la Forêt Silencieuse", "le Royaume Perdu", "la Ville Oubliée", "le Royaume Caché" } }
    };

    private static readonly Dictionary<string, string[]> LocalizedReviews = new()
    {
        { "en", new[] { "A masterpiece!", "Couldn't put it down.", "Highly recommend!", "Not what I expected." } },
        { "de", new[] { "Ein Meisterwerk!", "Konnte es nicht aus der Hand legen.", "Sehr empfehlenswert!", "Nicht das, was ich erwartet hatte." } },
        { "fr", new[] { "Un chef-d'œuvre!", "Impossible de le lâcher.", "Je le recommande vivement!", "Pas ce à quoi je m'attendais." } }
    };

    private static readonly Dictionary<string, string> LocalizedPublisherSuffix = new()
    {
        { "en", "Press" },
        { "de", "Verlag" },
        { "fr", "Éditions" }
    };

    public static List<Book> GenerateBooks(
        int count,
        string language,
        int seed,
        double avgLikes,
        double avgReviews,
        int startIndex = 1)
    {
        if (!SupportedLangs.Contains(language))
        {
            language = "en";
        }

        Randomizer.Seed = new Random(seed);

        var faker = new Faker(language);

        var books = new List<Book>(count);

        for (int i = 0; i < count; i++)
        {
            int index = startIndex + i;

            var prefix = faker.PickRandom(TitlePrefixes[language]);
            var suffix = faker.PickRandom(TitleSuffixes[language]);
            var title = $"{prefix} {suffix}";

            var authors = new List<string> { faker.Name.FullName() };
            if (faker.Random.Bool(0.3f))
            {
                authors.Add(faker.Name.FullName());
            }

            int finalLikes = WeightedRandomInt(avgLikes, faker);

            int reviewCount = WeightedRandomInt(avgReviews, faker);
            var reviews = new List<Review>();
            for (int r = 0; r < reviewCount; r++)
            {
                reviews.Add(new Review
                {
                    Author = faker.Name.FullName(),
                    Text = faker.PickRandom(LocalizedReviews[language])
                });
            }

            var publisherSuffix = LocalizedPublisherSuffix[language];
            var publisher = $"{faker.Company.CompanyName()} {publisherSuffix}";

            var book = new Book
            {
                Index = index,
                ISBN = faker.Random.Replace("978-#-###-#####-#"),
                Title = title,
                Authors = authors,
                Publisher = publisher,
                CoverUrl = GenerateCoverUrl(title, authors[0]),
                Likes = finalLikes,
                Reviews = reviews
            };

            books.Add(book);
        }

        return books;
    }

    private static int WeightedRandomInt(double average, Faker faker)
    {
        int floor = (int)Math.Floor(average);
        double fraction = average - floor;

        int result = floor;
        if (faker.Random.Double(0, 1) < fraction)
        {
            result = floor + 1;
        }
        return result;
    }

    private static string GenerateCoverUrl(string title, string author)
    {
        var seedInput = title + author;
        int hash = seedInput.GetHashCode();
        if (hash < 0)
            hash = -hash;
    
        return $"https://api.dicebear.com/8.x/shapes/svg?seed={hash}&size=200";
    }

}