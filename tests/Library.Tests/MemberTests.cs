using Library.Core;

namespace Library.Tests;

public class MemberTests
{
    private static (int MaxLoans, int LoanDays, decimal DailyFine) Rules(Member m) =>
        (m.MaxLoans, m.LoanDays, m.DailyFine);

    [Fact]
    public void Rules_DifferByMemberType()
    {
        Assert.Equal((3, 14, 0.50m), Rules(new StudentMember("S1", "Ama")));
        Assert.Equal((5, 30, 0.25m), Rules(new StaffMember("T1", "Kofi")));
        Assert.Equal((1, 7, 1.00m), Rules(new GuestMember("G1", "Esi")));
    }

    [Theory]
    [InlineData("", "Ama")]
    [InlineData("S1", " ")]
    public void Constructor_BlankIdOrName_Throws(string id, string name)
    {
        Assert.ThrowsAny<ArgumentException>(() => new StudentMember(id, name));
    }
}
