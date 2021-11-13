[![.NET 6](https://github.com/libanvl/opt/actions/workflows/dotnet.yml/badge.svg)](https://github.com/libanvl/opt/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/libanvl/opt/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/libanvl/opt/actions/workflows/codeql-analysis.yml)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/libanvl.opt?label=libanvl.opt)](https://www.nuget.org/packages/libanvl.opt/)

# libanvl.Opt

A null-free optional value library for .NET.

* Present values are represented as an Opt&lt;T&gt;.Some
* Missing values are represented as Opt&lt;T&gt;.None

## Requirements

[.NET 6](https://dotnet.microsoft.com/download/dotnet/6.0)

## Releases

* NuGet packages are available on [NuGet.org](https://www.nuget.org/packages/libanvl.opt)
  * Embedded debug symbols
  * Source Link enabled
* NuGet packages from CI builds are available on the [libanvl GitHub feed](https://github.com/libanvl/opt/packages/)

## Features

- [X] Immutable
- [X] Use Opt&lt;T&gt; instead of T? for optional values 
- [X] Implicit conversion from T to Opt&lt;T&gt;
- [X] Opts of IEnumerable&lt;T&gt; are iterable
- [X] Deep selection of properties in complex objects
- [X] SomeOrEmpty() for string and enumerables
- [X] SomeOrDefault() for any type
- [X] Change to a null with SomeOrNull()
- [X] Explicitly opt-in to exceptions with Unwrap()
- [X] Cast inner value to compatible type with Cast() 

## Examples

```csharp
class Car
{
	public string Driver { get; set;}
}

public void AcceptOptionalValue(Opt<Car> optCar, Opt<string> optName)
{
	if (optCar is Opt<Car>.Some someCar)
	{
		someCar.Value.Driver = optName.SomeOrDefault("Default Driver");
	}

	if (optCar.IsNone)
	{
		throw new Exception();
	}

	// or use Unwrap() to throw for None

	Car bcar = optCar.Unwrap();
}

public void RunCarOperations()
{
	var acar = new Car();
	AcceptOptionalValue(acar, "Rick");

	Car? nocar = null;
	AcceptOptionalValue(nocar.WrapOpt(), None.String)

	// use Select to project to an Opt of an inner property
	Opt<string> driver = acar.Select(x => x.Driver);
}

public void OptsOfEnumerablesAreIterable<T>(Opt<List<T>> optList)
{
	// if optList is None, the enumerable is empty, not null
	foreach (T item in optList)
	{
		Console.WriteLine(item);
	}

	// this is equivalent
	foreach (T item in optList.SomeOrEmpty())
	{
		Console.WriteLine(item);
	}
}
```
