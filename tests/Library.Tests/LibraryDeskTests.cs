using Library.Core;

namespace Library.Tests;

public class LibraryDeskTests
{
    private DateOnly _today = new(2026, 11, 24);
    private readonly LibraryDesk _desk;

    // xUnit builds a fresh instance per test, so every test starts from this setup.
    public LibraryDeskTests()
    {
        _desk = new LibraryDesk(() => _today);

        foreach (var (isbn, title) in new[] { ("111", "Dune"), ("222", "Emma"), ("333", "Ulysses"), ("444", "Beloved") })
        {
            _desk.AddBook(new Book(isbn, title, "Author"));
        }

        _desk.AddMember(new StudentMember("S1", "Ama"));
        _desk.AddMember(new GuestMember("G1", "Esi"));
    }

    [Fact]
    public void Borrow_ValidRequest_CreatesLoanDueAfterTheMembersPeriod()
    {
        var loan = _desk.Borrow("111", "S1");

        Assert.Equal(new DateOnly(2026, 12, 8), loan.DueOn);
        Assert.Single(_desk.ActiveLoans);
    }

    [Fact]
    public void Borrow_BookAlreadyOnLoan_Throws()
    {
        _desk.Borrow("111", "S1");

        var ex = Assert.Throws<LibraryException>(() => _desk.Borrow("111", "G1"));

        Assert.Contains("already on loan", ex.Message);
    }

    [Fact]
    public void Borrow_GuestOverTheirLimitOfOne_Throws()
    {
        _desk.Borrow("111", "G1");

        var ex = Assert.Throws<LibraryException>(() => _desk.Borrow("222", "G1"));

        Assert.Contains("at most 1", ex.Message);
    }

    [Fact]
    public void Borrow_StudentAllowedThreeButNotFour()
    {
        _desk.Borrow("111", "S1");
        _desk.Borrow("222", "S1");
        _desk.Borrow("333", "S1");

        Assert.Throws<LibraryException>(() => _desk.Borrow("444", "S1"));
    }

    [Fact]
    public void Borrow_MemberWithOverdueBook_Throws()
    {
        _desk.Borrow("111", "S1");
        _today = _today.AddDays(15); // due after 14 days

        var ex = Assert.Throws<LibraryException>(() => _desk.Borrow("222", "S1"));

        Assert.Contains("overdue", ex.Message);
    }

    [Theory]
    [InlineData("999", "S1")]
    [InlineData("111", "X9")]
    public void Borrow_UnknownBookOrMember_Throws(string isbn, string memberId)
    {
        Assert.Throws<LibraryException>(() => _desk.Borrow(isbn, memberId));
    }

    [Fact]
    public void Return_OnTime_NoFineAndBookBecomesAvailable()
    {
        _desk.Borrow("111", "S1");

        var receipt = _desk.Return("111");

        Assert.Equal(0m, receipt.Fine);
        Assert.False(_desk.IsOnLoan("111"));
        Assert.Empty(_desk.ActiveLoans);
    }

    [Fact]
    public void Return_ThreeDaysLate_ChargesStudentRate()
    {
        _desk.Borrow("111", "S1");
        _today = new DateOnly(2026, 12, 11); // three days after the 8 Dec due date

        Assert.Equal(1.50m, _desk.Return("111").Fine);
    }

    [Fact]
    public void Return_BookNotOnLoan_Throws()
    {
        Assert.Throws<LibraryException>(() => _desk.Return("111"));
    }

    [Fact]
    public void AddBook_DuplicateIsbn_Throws()
    {
        Assert.Throws<LibraryException>(() => _desk.AddBook(new Book("111", "Other", "Author")));
    }

    [Fact]
    public void AddMember_DuplicateId_Throws()
    {
        Assert.Throws<LibraryException>(() => _desk.AddMember(new GuestMember("S1", "Kofi")));
    }
}
