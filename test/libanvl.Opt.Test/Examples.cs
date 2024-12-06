using libanvl.Exceptions;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace libanvl.Test;

public class Examples
{
    [Fact]
    public void WrapOpt()
    {
        var rick = new Person("Rick", "Sanchez", Org.Alpha);
        var morty = new Person("Mortimer", "Smith", Org.Gamma);

        Opt<Person> optPerson = Opt.From(rick);
        Assert.True(optPerson.IsSome);
        var person = optPerson.Unwrap();

        if (optPerson.IsSome)
        {
            Assert.Same(person, optPerson.Unwrap());
        }

        Assert.Same(person, optPerson.SomeOr(morty));
        Assert.NotNull(optPerson.SomeOrDefault());

        optPerson = Opt.From<Person>(null);
        Assert.True(optPerson.IsNone);
        Assert.Throws<OptException>(optPerson.Unwrap);
        Assert.Same(morty, optPerson.SomeOr(morty));
        Assert.Null(optPerson.SomeOrDefault());

        static bool AcceptOpt(Opt<Person> op) => op.IsSome;

        // implicit conversion
        var result = AcceptOpt(rick);
        Assert.True(result);

        // explicit Some factory
        result = AcceptOpt(Opt.Some(rick));
        Assert.True(result);

        // extension function factory
        result = AcceptOpt(rick);
        Assert.True(result);

        // This will not work:
        // result = AcceptOpt(null);

        // explicit None
        result = AcceptOpt(Opt<Person>.None);
        Assert.False(result);
    }

    [Fact]
    public void WrapOptAndProject()
    {
        static Opt<DirectoryInfo> GetOptDirectoryInfo(string? path) => Opt.From(path is null ? null : new DirectoryInfo(path));

        Opt<DirectoryInfo> optDirectoryInfo = GetOptDirectoryInfo(@"C:\Users");
        Assert.True(optDirectoryInfo.IsSome);

        optDirectoryInfo = GetOptDirectoryInfo(null);
        Assert.True(optDirectoryInfo.IsNone);
    }

    [Fact]
    public void SelectThroughOpt()
    {
        var rick = new Person("Rick", "Sanchez", Org.Alpha);
        var morty = new Person("Mortimer", "Smith", Org.Gamma);

        var optBook = Opt.From(new Book("How to drive a space car", rick, morty));

        Opt<string> optEditorLastName = optBook.Select(b => b.Editor.LastName);
        Assert.Equal(morty.LastName, optEditorLastName.Unwrap());

        optBook = Opt<Book>.None;
        optEditorLastName = optBook.Select(b => b.Editor.LastName);
        Assert.True(optEditorLastName.IsNone);
    }

    [Fact]
    public void IterateOptEnumerable()
    {
        var rick = new Person("Rick", "Sanchez", Org.Alpha);
        var morty = new Person("Mortimer", "Smith", Org.Alpha);
        var beth = new Person("Beth", "Smit", Org.Delta);

        var book1 = new Book("Trans-dimensional Family Dynamics", beth, morty);
        var book2 = new Book("Horse Surgeon: A Life", beth, morty);

        var books = new List<Book> { book1, book2 };
        var library = new Library(books, rick);

        foreach (Book b in library.Books.Unwrap())
        {
            Assert.Same(b.Editor, morty);
        }

        library = new Library(Opt<IEnumerable<Book>>.None, Opt<Person>.None);

        // Books will be an empty collection
        foreach (Book b in library.Books)
        {
            Assert.Fail("Unreachable");
        }
    }

    [Fact]
    public void CastThroughOpt()
    {
        // implicit conversion to Opt
        Opt<DeltaPerson> optJerry = new DeltaPerson("Jerry", "Smith");
        Opt<Person> optPerson = optJerry.Cast<Person>();

        Assert.Same(optJerry.Unwrap(), optPerson.Unwrap());

        Opt<Person> optVariantPerson = optJerry.Cast<Person>();
    }
}