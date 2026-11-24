namespace Library.Core;

/// <summary>A title in the catalogue. Equality is by value, so two Books with the same details are the same book.</summary>
public sealed record Book
{
    public Book(string isbn, string title, string author)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(isbn);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(author);

        Isbn = isbn.Trim();
        Title = title.Trim();
        Author = author.Trim();
    }

    public string Isbn { get; }
    public string Title { get; }
    public string Author { get; }
}
