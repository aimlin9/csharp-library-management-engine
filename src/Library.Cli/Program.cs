using Library.Cli;
using Library.Core;

var desk = new LibraryDesk(() => DateOnly.FromDateTime(DateTime.Today));

// A little starter data so the app can be tried straight away.
desk.AddBook(new Book("111", "Dune", "Frank Herbert"));
desk.AddBook(new Book("222", "Things Fall Apart", "Chinua Achebe"));
desk.AddMember(new StudentMember("S1", "Ama"));
desk.AddMember(new GuestMember("G1", "Esi"));

new LibraryApp(desk, Console.In, Console.Out).Run();
