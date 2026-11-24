using Library.Core;

namespace Library.Tests;

public class BookTests
{
    [Theory]
    [InlineData("", "Dune", "Herbert")]
    [InlineData("111", "", "Herbert")]
    [InlineData("111", "Dune", "  ")]
    public void Constructor_BlankField_Throws(string isbn, string title, string author)
    {
        Assert.ThrowsAny<ArgumentException>(() => new Book(isbn, title, author));
    }
}
