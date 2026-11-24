using Library.Core;

namespace Library.Tests;

public class LoanTests
{
    private static readonly DateOnly Start = new(2026, 11, 24);
    private static readonly Book Sample = new("111", "Dune", "Herbert");

    public static TheoryData<int, decimal> StudentFines => new()
    {
        { -3, 0m },   // early
        { 0, 0m },    // on the due date is not late
        { 4, 2.00m }, // 4 days x 0.50
    };

    [Fact]
    public void DueOn_UsesTheMembersLoanPeriod()
    {
        Assert.Equal(new DateOnly(2026, 12, 8), new Loan(Sample, new StudentMember("S1", "Ama"), Start).DueOn);
        Assert.Equal(new DateOnly(2026, 12, 24), new Loan(Sample, new StaffMember("T1", "Kofi"), Start).DueOn);
        Assert.Equal(new DateOnly(2026, 12, 1), new Loan(Sample, new GuestMember("G1", "Esi"), Start).DueOn);
    }

    [Theory]
    [MemberData(nameof(StudentFines))]
    public void FineFor_Student_ChargesPerDayLateOnly(int daysLate, decimal expected)
    {
        var loan = new Loan(Sample, new StudentMember("S1", "Ama"), Start);

        Assert.Equal(expected, loan.FineFor(loan.DueOn.AddDays(daysLate)));
    }

    [Fact]
    public void FineFor_Guest_UsesTheHigherDailyRate()
    {
        var loan = new Loan(Sample, new GuestMember("G1", "Esi"), Start);

        Assert.Equal(2.00m, loan.FineFor(loan.DueOn.AddDays(2)));
    }
}
