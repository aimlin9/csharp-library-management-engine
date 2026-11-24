namespace Library.Core;

/// <summary>A book on loan to a member.</summary>
public sealed record Loan(Book Book, Member Member, DateOnly BorrowedOn)
{
    public DateOnly DueOn => BorrowedOn.AddDays(Member.LoanDays);

    /// <summary>Fine for returning on the given date. Early or on-time returns cost nothing.</summary>
    public decimal FineFor(DateOnly returnedOn)
    {
        var daysLate = returnedOn.DayNumber - DueOn.DayNumber;

        return daysLate > 0 ? daysLate * Member.DailyFine : 0m;
    }
}
