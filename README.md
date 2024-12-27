[![CI](https://github.com/libanvl/opt/actions/workflows/libanvl-dotnet-ci.yml/badge.svg?branch=main)](https://github.com/libanvl/opt/actions/workflows/libanvl-dotnet-ci.yml)
[![CodeQL](https://github.com/libanvl/opt/actions/workflows/github-code-scanning/codeql/badge.svg?branch=main)](https://github.com/libanvl/opt/actions/workflows/github-code-scanning/codeql)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/libanvl.opt?label=libanvl.opt)](https://www.nuget.org/packages/libanvl.opt/)
[![codecov](https://codecov.io/gh/libanvl/opt/graph/badge.svg?token=X29VU1I53I)](https://codecov.io/gh/libanvl/opt)

# libanvl.Opt

A null-free optional value library for .NET.

* An optional value is represented as the struct Opt&lt;T&gt;
* A possible value or error is represented as the struct Result&lt;T, E&gt;

See the [Examples Tests](test/libanvl.Opt.Test/Examples.cs) for more on how to use Opt.

## Requirements

[.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)

## Releases

* NuGet packages are available on [NuGet.org](https://www.nuget.org/packages/libanvl.opt)
  * Embedded debug symbols
  * Source Link enabled
* NuGet packages from CI builds are available on the [libanvl GitHub feed](https://github.com/libanvl/opt/packages/)

## libanvl.Opt Features

- [X] Immutable
- [X] Use Opt&lt;T&gt; instead of T? for optional values 
- [X] Implicit conversion from T to Opt&lt;T&gt;
- [X] Deep selection of properties in complex objects
- [X] SomeOrDefault() for any type
- [X] Explicitly opt-in to exceptions with Unwrap()
- [X] Cast inner value to compatible type with Cast&lt;U&gt;() 
- [ ] Opts of IEnumerable&lt;T&gt; are iterable

## libanvl.Result Features

- [X] Create success results with Result.Ok
- [X] Create error results with Result.Err
- [X] Unwrap values with Unwrap, throwing if the result is an error
- [X] Match on success or error with Match
- [X] Convert between Opt and Result

## libanvl.OneOrMany Features

- [X] OneOrMany&lt;T&gt; for a single value or a collection of values
- [X] Implicit conversions
- [X] Equality operators
- [X] Minimizes allocations

## Examples

```csharp
class Car
{
  public string Driver { get; set;}
}

public void AcceptOptionalValue(Opt<Car> optCar, Opt<string> optName)
{
  if (optCar.IsSome)
  {
    optCar.Unwrap().Driver = optName.SomeOr("Default Driver");
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
  AcceptOptionalValue(Opt.From(nocar), Opt<string>.None)

  // use Select to project to an Opt of an inner property
  Opt<string> driver = acar.Select(x => x.Driver);
}

public void RunResultOperations()
{
  Result<Car, string> carResult = Result.Ok(new Car());
  Result<Car, string> errorResult = Result.Err("Error");

  carResult.Match(
    Ok: car => Console.WriteLine(car.Driver),
    Err: err => Console.WriteLine(err)
  );

  // Convert between Opt and Result
  Opt<Car> optCar = carResult.ToOpt();
  Result<Car, string> resultCar = optCar.ToResult("Error");
}
```
