using libanvl.Exceptions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xunit;

namespace libanvl.opt.test;

public class Examples
{
    [Fact]
    public void WrapOpt()
    {
        var rick = new Person("Rick", "Sanchez", Org.Alpha);
        var morty = new Person("Mortimer", "Smith", Org.Gamma);

        var optPerson = Opt.From(rick);
        Assert.True(optPerson.IsSome);
        var person = optPerson.Unwrap();

        if (optPerson.IsSome)
            Assert.Same(person, optPerson.Unwrap());

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
    }

    [Fact]
    public void Operator_ImplicitConversion_ToOpt()
    {
        Opt<int> opt = 5;
        Assert.True(opt.IsSome);
        Assert.Equal(5, opt.Unwrap());
    }

    [Fact]
    public void Operator_ImplicitConversion_FromOpt()
    {
        Opt<int> opt = Opt.Some(5);
        int value = opt;
        Assert.Equal(5, value);
    }

    [Fact]
    public void Operator_True()
    {
        Opt<int> opt = 5;
        if (opt)
        {
            Assert.True(opt.IsSome);
        }
        else
        {
            Assert.Fail("Should not be called");
        }
    }

    [Fact]
    public void Operator_False()
    {
        Opt<int> opt = Opt<int>.None;
        if (opt)
        {
            Assert.Fail("Should not be called");
        }
        else
        {
            Assert.True(opt.IsNone);
        }
    }

    [Fact]
    public void Operator_Or_Opt()
    {
        Opt<int> opt1 = Opt<int>.None;
        Opt<int> opt2 = 5;
        Opt<int> result = opt1 | opt2;
        Assert.True(result.IsSome);
        Assert.Equal(5, result.Unwrap());
    }

    [Fact]
    public void Operator_Or_Value()
    {
        Opt<int> opt = Opt<int>.None;
        int result = opt | 5;
        Assert.Equal(5, result);
    }

    [Fact]
    public void Result_Ok()
    {
        var result = Result.Ok("Success");
        Assert.True(result.IsOk);
        Assert.False(result.IsErr);
        Assert.Equal("Success", result.Unwrap());
    }

    [Fact]
    public void Result_Err()
    {
        var result = Result.Err<string, string>("Error");
        Assert.False(result.IsOk);
        Assert.True(result.IsErr);
        Assert.Throws<InvalidOperationException>(() => result.Unwrap());
    }

    [Fact]
    public void Result_Match_Ok()
    {
        var result = Result.Ok("Success");
        result.Match(
            ok => Assert.Equal("Success", ok),
            err => Assert.Fail("Should not be called")
        );
    }

    [Fact]
    public void Result_Match_Err()
    {
        var result = Result.Err<string, string>("Error");
        result.Match(
            ok => Assert.Fail("Should not be called"),
            err => Assert.Equal("Error", err)
        );
    }

    [Fact]
    public void Result_OkOr()
    {
        var result = Result.Err<string, string>("Error");
        var value = result.OkOr("Default");
        Assert.Equal("Default", value);
    }

    [Fact]
    public void Result_OkOrDefault()
    {
        var result = Result.Err<string, string>("Error");
        var value = result.OkOrDefault();
        Assert.Null(value);
    }

    [Fact]
    public void Result_ImplicitConversion_Ok()
    {
        Result<string, Exception> result = "Success";
        Assert.True(result.IsOk);
        Assert.Equal("Success", result.Unwrap());
    }

    [Fact]
    public void Result_ImplicitConversion_Err()
    {
        Result<int, string> result = "Error";
        Assert.True(result.IsErr);
        Assert.Throws<InvalidOperationException>(() => result.Unwrap());
    }

    [Fact]
    public void Any_None()
    {
        Any<int> none = default;
        Assert.True(none.IsNone);
    }

    [Fact]
    public void Any_Single()
    {
        Any<int> single = 5;
        Assert.True(single.IsSingle);
        Assert.Equal(5, single.Single.Unwrap());
    }

    [Fact]
    public void Any_Many()
    {
        Any<int> many = [1, 2, 3];
        Assert.True(many.IsMany);
        Assert.Equal(3, many.Count);
        Assert.Collection(
            many.Many.Unwrap(),
            x => Assert.Equal(1, x),
            x => Assert.Equal(2, x),
            x => Assert.Equal(3, x));
    }

    [Fact]
    public void Any_Iterate()
    {
        Any<int> none = default;
        foreach (int i in none)
        {
            Assert.Fail("Unreachable");
        }

        Any<int> single = 5;
        var sum = 0;
        foreach (int i in single)
        {
            sum += i;
        }

        Any<int> many = [1, 2, 3];
        sum = 0;
        foreach (int i in many)
        {
            sum += i;
        }
        Assert.Equal(6, sum);
    }

#if NET8_0_OR_GREATER

    [Fact]
    public void MapOfAny()
    {
        AnyMap<string, int> map = new()
        {
            ["alpha"] = default,
            ["bravo"] = 2,
            ["charlie"] = [3, 4, 5]
        };

        Assert.True(map.ContainsKey("alpha"));
        Assert.True(map.ContainsKey("bravo"));
        Assert.True(map.ContainsKey("charlie"));

        IAnyRefMap<string, int> refMap = map;
        ref var value = ref refMap.GetValueRef("alpha");
        Assert.True(value.IsNone);

        value = ref refMap.GetValueRef("bravo");
        Assert.True(value.IsSingle);
        Assert.Equal(2, value.Single.Unwrap());

        value = ref refMap.GetValueRef("charlie");
        Assert.True(value.IsMany);
        Assert.Collection(
            value.Many.Unwrap(),
            x => Assert.Equal(3, x),
            x => Assert.Equal(4, x),
            x => Assert.Equal(5, x));

        refMap["alpha"].Add(1);
        Assert.True(map["alpha"].IsSingle);
        Assert.Equal(1, map["alpha"].Single.Unwrap());

        refMap["bravo"].Add(3);
        Assert.True(map["bravo"].IsMany);
        Assert.Collection(
            map["bravo"].Many.Unwrap(),
            x => Assert.Equal(2, x),
            x => Assert.Equal(3, x));
    }
#endif
}
