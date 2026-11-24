using Library.Core;

namespace Library.Cli;

/// <summary>Menu and prompts only. All rules live in LibraryDesk. Input and output are injected so tests can script a session.</summary>
public sealed class LibraryApp(LibraryDesk desk, TextReader input, TextWriter output)
{
    public void Run()
    {
        output.WriteLine("Library Management");

        try
        {
            while (true)
            {
                ShowMenu();
                var choice = Ask("Choose (1-8): ");

                if (choice == "8")
                {
                    output.WriteLine("Goodbye!");
                    return;
                }

                Dispatch(choice);
            }
        }
        catch (EndOfStreamException)
        {
            output.WriteLine();
            output.WriteLine("Input ended. Goodbye!");
        }
    }

    private void Dispatch(string choice)
    {
        try
        {
            switch (choice)
            {
                case "1": AddBook(); break;
                case "2": AddMember(); break;
                case "3": Borrow(); break;
                case "4": Return(); break;
                case "5": ListBooks(); break;
                case "6": ListMembers(); break;
                case "7": ShowLoans(); break;
                default: output.WriteLine("Please choose a number from 1 to 8."); break;
            }
        }
        catch (LibraryException ex)
        {
            output.WriteLine($"Error: {ex.Message}");
        }
        catch (ArgumentException)
        {
            // Book and Member constructors only throw this for blank fields.
            output.WriteLine("Error: every field is required.");
        }
    }

    private void AddBook()
    {
        desk.AddBook(new Book(Ask("ISBN: "), Ask("Title: "), Ask("Author: ")));
        output.WriteLine("Book added.");
    }

    private void AddMember()
    {
        var type = Ask("Type (1 Student, 2 Staff, 3 Guest): ");
        var id = Ask("Member ID: ");
        var name = Ask("Name: ");

        desk.AddMember(type switch
        {
            "1" => new StudentMember(id, name),
            "2" => new StaffMember(id, name),
            "3" => new GuestMember(id, name),
            _ => throw new LibraryException("Type must be 1, 2 or 3."),
        });
        output.WriteLine("Member added.");
    }

    private void Borrow()
    {
        var loan = desk.Borrow(Ask("ISBN: "), Ask("Member ID: "));
        output.WriteLine($"Borrowed \"{loan.Book.Title}\". Due {loan.DueOn:yyyy-MM-dd}.");
    }

    private void Return()
    {
        var receipt = desk.Return(Ask("ISBN: "));
        output.WriteLine($"Returned \"{receipt.Loan.Book.Title}\". Fine: {receipt.Fine:0.00}");
    }

    private void ListBooks()
    {
        foreach (var book in desk.Books)
        {
            var status = desk.IsOnLoan(book.Isbn) ? "On loan" : "Available";
            output.WriteLine($"  {book.Isbn} | {book.Title} by {book.Author} | {status}");
        }
    }

    private void ListMembers()
    {
        foreach (var member in desk.Members)
        {
            output.WriteLine($"  {member.Id} | {member.Name} | {member.Kind}");
        }
    }

    private void ShowLoans()
    {
        var loans = desk.LoansFor(Ask("Member ID: "));

        if (loans.Count == 0)
        {
            output.WriteLine("No active loans.");
        }

        foreach (var loan in loans)
        {
            output.WriteLine($"  {loan.Book.Title} | due {loan.DueOn:yyyy-MM-dd}");
        }
    }

    private void ShowMenu()
    {
        output.WriteLine();
        output.WriteLine("1. Add book   2. Add member   3. Borrow   4. Return");
        output.WriteLine("5. List books 6. List members 7. Member loans   8. Quit");
    }

    // A null line means the input stream has closed for good.
    private string Ask(string prompt)
    {
        output.Write(prompt);

        return input.ReadLine() ?? throw new EndOfStreamException();
    }
}
