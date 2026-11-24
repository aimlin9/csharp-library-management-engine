namespace Library.Core;

/// <summary>A library member. Subclasses define their own borrowing rules.</summary>
public abstract class Member
{
    protected Member(string id, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = id.Trim();
        Name = name.Trim();
    }

    public string Id { get; }
    public string Name { get; }

    public abstract string Kind { get; }
    public abstract int MaxLoans { get; }
    public abstract int LoanDays { get; }
    public abstract decimal DailyFine { get; }
}

public sealed class StudentMember(string id, string name) : Member(id, name)
{
    public override string Kind => "Student";
    public override int MaxLoans => 3;
    public override int LoanDays => 14;
    public override decimal DailyFine => 0.50m;
}

public sealed class StaffMember(string id, string name) : Member(id, name)
{
    public override string Kind => "Staff";
    public override int MaxLoans => 5;
    public override int LoanDays => 30;
    public override decimal DailyFine => 0.25m;
}

public sealed class GuestMember(string id, string name) : Member(id, name)
{
    public override string Kind => "Guest";
    public override int MaxLoans => 1;
    public override int LoanDays => 7;
    public override decimal DailyFine => 1.00m;
}
