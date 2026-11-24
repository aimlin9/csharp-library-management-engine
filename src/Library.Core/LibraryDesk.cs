namespace Library.Core;

/// <summary>Thrown when a request breaks a library rule. The message is written for the user.</summary>
public sealed class LibraryException(string message) : Exception(message);

public sealed record ReturnReceipt(Loan Loan, DateOnly ReturnedOn, decimal Fine);

/// <summary>Holds the catalogue, the members and the active loans, and enforces the borrowing rules.</summary>
/// <param name="today">Supplies today's date, so tests can control time.</param>
public sealed class LibraryDesk(Func<DateOnly> today)
{
    private readonly Dictionary<string, Book> _books = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Member> _members = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<Loan> _loans = [];

    public IReadOnlyCollection<Book> Books => _books.Values;
    public IReadOnlyCollection<Member> Members => _members.Values;
    public IReadOnlyList<Loan> ActiveLoans => _loans.AsReadOnly();

    public void AddBook(Book book)
    {
        if (!_books.TryAdd(book.Isbn, book))
        {
            throw new LibraryException($"A book with ISBN {book.Isbn} already exists.");
        }
    }

    public void AddMember(Member member)
    {
        if (!_members.TryAdd(member.Id, member))
        {
            throw new LibraryException($"A member with ID {member.Id} already exists.");
        }
    }

    public IReadOnlyList<Loan> LoansFor(string memberId) =>
        _loans.Where(l => string.Equals(l.Member.Id, memberId.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();

    public bool IsOnLoan(string isbn) =>
        _loans.Any(l => string.Equals(l.Book.Isbn, isbn.Trim(), StringComparison.OrdinalIgnoreCase));

    public Loan Borrow(string isbn, string memberId)
    {
        var book = Find(_books, isbn, "book");
        var member = Find(_members, memberId, "member");
        var date = today();

        if (IsOnLoan(book.Isbn))
        {
            throw new LibraryException($"\"{book.Title}\" is already on loan.");
        }

        var current = LoansFor(member.Id);

        // Checked before the limit so the member learns the more actionable problem first.
        if (current.Any(l => l.DueOn < date))
        {
            throw new LibraryException($"{member.Name} has overdue books and must return them first.");
        }

        if (current.Count >= member.MaxLoans)
        {
            throw new LibraryException($"{member.Name} ({member.Kind}) can borrow at most {member.MaxLoans} book(s) at a time.");
        }

        var loan = new Loan(book, member, date);
        _loans.Add(loan);

        return loan;
    }

    public ReturnReceipt Return(string isbn)
    {
        var loan = _loans.FirstOrDefault(l => string.Equals(l.Book.Isbn, isbn.Trim(), StringComparison.OrdinalIgnoreCase))
            ?? throw new LibraryException("That book is not currently on loan.");

        var date = today();
        _loans.Remove(loan);

        return new ReturnReceipt(loan, date, loan.FineFor(date));
    }

    private static T Find<T>(Dictionary<string, T> items, string key, string what) =>
        items.TryGetValue(key.Trim(), out var item)
            ? item
            : throw new LibraryException($"No {what} with ID {key.Trim()} was found.");
}
