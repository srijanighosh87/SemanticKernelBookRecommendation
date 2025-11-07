using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using SemanticKernelExample.Plugins.BookRating;
using System.Diagnostics;
using System.Text.Json;

// setup kernel with Azure OpenAI
var builder = Kernel.CreateBuilder();
var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var endpoint = config["AzureOpenAI:endpoint"];
if (string.IsNullOrEmpty(endpoint))
{
    throw new InvalidOperationException("AzureOpenAI endpoint configuration is missing.");
}
var deploymentName = config["AzureOpenAI:deploymentName"];
if (string.IsNullOrEmpty(deploymentName))
{
    throw new InvalidOperationException("AzureOpenAI deploymentName configuration is missing.");
}
var apiKey = config["AzureOpenAI:apiKey"];
if (string.IsNullOrEmpty(apiKey))
{
    throw new InvalidOperationException("AzureOpenAI apiKey configuration is missing.");
}

builder.AddAzureOpenAIChatCompletion(
    deploymentName: deploymentName,
    endpoint: endpoint,
    apiKey: apiKey
);

// Book Recommendation Plugin - filtered by rating obtained from google books API.
builder.Plugins.AddFromType<BookSearch>();
var kernel = builder.Build();
var plugins = kernel.CreatePluginFromPromptDirectory("Plugins");

Console.WriteLine("=== AI Book Recommendations with Rating Filter ===\n");
Console.WriteLine("What genre do you enjoy reading?\n");
var userInput = Console.ReadLine();
Console.WriteLine();

var validatedBooks = new List<string>();

Console.WriteLine($"Getting recommendations...\n");
var aiRecommendations = await kernel.InvokeAsync(
        plugins["BookRecommendation"],
        new KernelArguments { ["userInput"] = userInput }
    );

var jsonResponse = aiRecommendations.ToString();
var books = JsonSerializer.Deserialize<List<BookRecommendation>>(jsonResponse);
if (books == null || books.Count == 0)
{
Console.WriteLine("No book recommendations found.");
return;
}

foreach (var book in books)
{
var ratingResult = await kernel.InvokeAsync(
    "BookSearch",
    "GetBookByTitleAndMinimumRating",
    new KernelArguments { ["title"] = book.Title, ["author"] = book.Author, ["minRating"] = 4.0 }
);
// Add only books that meet the rating criteria
if (!ratingResult.ToString().Contains("No books found"))
{
validatedBooks.Add($"Title: {book.Title} {ratingResult}\nAuthor: {book.Author}\nDescription:\n {book.Description}");
}
}

Console.WriteLine("=== Here is the Book Recommendations for you ===\n");
foreach (var book in validatedBooks)
{
Console.WriteLine(book);
Console.WriteLine();
}

/// <summary>
/// Represents a recommendation for a book, including its title, author, and description.
/// </summary>
public class BookRecommendation
{
    public required string Title { get; set; }
    public required string Author { get; set; }
    public string? Description { get; set; }
}
