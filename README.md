# Library Management Engine (C#)

![CI](https://github.com/aimlin9/csharp-library-management-engine/actions/workflows/ci.yml/badge.svg)

A console library system built with C# and .NET 10. It handles books, members, borrowing, returns, due dates, fines and validation, with different borrowing rules for each type of member.

## Run it

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

```bash
git clone https://github.com/aimlin9/csharp-library-management-engine.git
cd csharp-library-management-engine
dotnet run --project src/Library.Cli
dotnet test
```

The app starts with two books (ISBN `111`, `222`) and two members (`S1` student, `G1` guest) so you can try it immediately. In Visual Studio 2026, open `LibraryManagement.slnx`, set **Library.Cli** as the startup project and press F5.

## Borrowing rules

| Member type | Max loans | Loan period | Fine per day late |
|---|---|---|---|
| Student | 3 | 14 days | 0.50 |
| Staff | 5 | 30 days | 0.25 |
| Guest | 1 | 7 days | 1.00 |

A borrow is refused if the book does not exist, is already on loan, the member does not exist, the member has an overdue book, or the member is at their limit.

## Class design

| Class | Role |
|---|---|
| `Book` (record) | A catalogue entry. Validated on creation; equal by value. |
| `Member` (abstract) | Shared id and name, plus abstract `MaxLoans`, `LoanDays` and `DailyFine`. |
| `StudentMember`, `StaffMember`, `GuestMember` | Each supplies its own rules by overriding the abstract members. |
| `Loan` (record) | Links a book and member to a borrow date. Works out its own due date and fine from the member's rules. |
| `LibraryDesk` | The engine. Owns books, members and active loans, and enforces every rule. |
| `LibraryException` | Raised for rule violations, with a message written for the user. |
| `LibraryApp` (Cli) | Menu and prompts only. Contains no rules. |

Why it is built this way:

- **Inheritance and polymorphism:** `LibraryDesk` asks a member for `MaxLoans` and never checks which type it is. Adding a new member type means adding one small class, with no edits to the engine.
- **Composition:** a `Loan` holds a `Member`, so the due date and fine always follow that member's rules and cannot disagree with them.
- **Encapsulation:** the collections are private and exposed read-only, so loans can only change through `Borrow` and `Return`.
- **Separation of concerns:** `Library.Core` has no console code, so every rule is unit tested directly. `LibraryApp` takes a `TextReader` and `TextWriter`, so whole sessions are tested with scripted input.
- **Injected clock:** `LibraryDesk` receives today's date as a function, so tests can move time forward to check overdue and fine behaviour.

## Testing

27 xUnit tests cover validation, the rules for each member type, loan limits, overdue blocking, due dates, fines (early, on time, late), duplicate and unknown ids, and scripted console sessions including bad input. They run with `dotnet test` and on GitHub for every push and pull request.

## Assumptions

- Each ISBN is a single copy.
- ISBNs and member IDs are trimmed and case-insensitive. ISBN format is not checked.
- Returning on the due date is not late. Each full day after it adds one daily fine.
- A member with any overdue loan cannot borrow until it is returned.
- Fines are calculated and shown on return. They are not stored or collected, and an unpaid fine does not block borrowing.
- Fine amounts use one unspecified currency.

## Known limitations

- Everything is in memory and is lost when the app closes.
- Books and members cannot be edited or removed.
- No renewals, reservations or loan history after a return.
- Single user, English only, console only.
