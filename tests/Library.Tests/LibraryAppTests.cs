using Library.Cli;
using Library.Core;

namespace Library.Tests;

public class LibraryAppTests
{
    private static string Run(params string[] lines)
    {
        var desk = new LibraryDesk(() => new DateOnly(2026, 11, 24));
        var output = new StringWriter();

        new LibraryApp(desk, new StringReader(string.Join(Environment.NewLine, lines)), output).Run();

        return output.ToString();
    }

    [Fact]
    public void FullSession_AddBorrowReturn_ShowsDueDateAndReceipt()
    {
        var output = Run(
            "1", "111", "Dune", "Herbert",
            "2", "1", "S1", "Ama",
            "3", "111", "S1",
            "4", "111",
            "8");

        Assert.Contains("Due 2026-12-08", output);
        Assert.Contains("Returned \"Dune\"", output);
    }

    [Fact]
    public void RuleViolation_ShowsErrorAndKeepsGoing()
    {
        var output = Run("3", "999", "S1", "8");

        Assert.Contains("Error: No book with ID 999", output);
        Assert.Contains("Goodbye!", output);
    }

    [Fact]
    public void BadMenuChoiceAndBlankField_AreHandledWithoutCrashing()
    {
        var output = Run("abc", "1", "", "Title", "Author", "8");

        Assert.Contains("Please choose a number from 1 to 8.", output);
        Assert.Contains("every field is required", output);
        Assert.Contains("Goodbye!", output);
    }

    [Fact]
    public void InputEndingMidPrompt_ExitsCleanly()
    {
        Assert.Contains("Input ended.", Run("3"));
    }
}
