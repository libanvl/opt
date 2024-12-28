[![CI](https://github.com/libanvl/opt/actions/workflows/libanvl-dotnet-ci.yml/badge.svg?branch=main)](https://github.com/libanvl/opt/actions/workflows/libanvl-dotnet-ci.yml)
[![CodeQL](https://github.com/libanvl/opt/actions/workflows/github-code-scanning/codeql/badge.svg?branch=main)](https://github.com/libanvl/opt/actions/workflows/github-code-scanning/codeql)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/libanvl.opt?label=libanvl.opt)](https://www.nuget.org/packages/libanvl.opt/)
[![codecov](https://codecov.io/gh/libanvl/opt/graph/badge.svg?token=X29VU1I53I)](https://codecov.io/gh/libanvl/opt)

# libanvl.Opt

A null-free optional value library for .NET with an emphasis on minimizing additional allocations.

* An optional value is represented as the struct Opt&lt;T&gt;
* A possible value or error is represented as the struct Result&lt;T, E&gt;
* A zero to N values are represented as the struct Any&lt;T&gt;

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

## libanvl.Any Features

- [X] Any&lt;T&gt; for a single value or multiple values
- [X] Implicit conversions
- [X] Equality operators
- [X] Add and remove elements
- [X] Convert to array, list, or enumerable
- [X] Convert to Opt&lt;T&gt;
- [X] Cast elements to a compatible type