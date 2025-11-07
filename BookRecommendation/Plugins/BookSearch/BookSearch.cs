using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SemanticKernelExample.Plugins.BookRating;

public class BookSearch
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private const string GOOGLE_BOOKS_API = "https://www.googleapis.com/books/v1/volumes";

    [KernelFunction, Description("Search for books by title with a minimum rating")]
    public async Task<string> GetBookByTitleAndMinimumRating(
        [Description("The title of the book to search for")] string title,
        [Description("The author of the book")] string author,
        [Description("Minimum rating (1-5")] double minRating = 4.0)
    {
        var url = $"{GOOGLE_BOOKS_API}?q=intitle:{title}+inauthor:{author}&maxResults=1"; var response = await _httpClient.GetStringAsync(url);

        var jsonDoc = JsonDocument.Parse(response);
        var items = jsonDoc.RootElement.GetProperty("items");

        var results = new List<string>();
        var seenBooks = new HashSet<string>();
        foreach (var item in items.EnumerateArray())
        {
            var volumeInfo = item.GetProperty("volumeInfo");

            // Get rating 
            if (!volumeInfo.TryGetProperty("averageRating", out var ratingElement))
            {
                continue;
            }

            var rating = ratingElement.GetDouble();

            // Filter by minimum rating
            if (rating < minRating)
            {
                continue;
            }

            var bookTitle = volumeInfo.TryGetProperty("title", out var t) ? t.GetString() : "Unknown";
            var authors = volumeInfo.TryGetProperty("authors", out var a) && a.GetArrayLength() > 0
                ? string.Join(", ", a.EnumerateArray().Select(x => x.GetString()))
                : "Unknown Author";
            var ratingsCount = volumeInfo.TryGetProperty("ratingsCount", out var rc) ? rc.GetInt32() : 0;

            // Create unique identifier for the book
            var bookIdentifier = $"{bookTitle}|{authors}".ToLowerInvariant();

            // Check if this book has already been added
            if (seenBooks.Contains(bookIdentifier))
            {
                continue;
            }

            seenBooks.Add(bookIdentifier);
            results.Add($"Rating: {rating}/5.");
        }

        return results.Count > 0
            ? string.Join("\n\n", results)
            : $"No books found with rating above {minRating}.";
    }
}
