# CSharp-function-overloading

## Problem:

Expose a single C# API taking a query string and zero or more optional parameters that has a return value that depends on the type parameter, and that can select different implementations for different kinds of types at compile time.

You may not use the `dynamic` type. The implementation for `class` must be able to do `new T()`.

The following four sets of type parameters must invoke four different implementations:
 * All non-nullable value types (`int, char, double...`)
 * The nullable string value type (`string`)
 * All class objects with a default constructor (`class`)
 * A generic catch-all implementation that can handle `Nullable<T>`
   and also accepts any number of parameters.

Essentially, the exposed API looks like this:

```cs
SQLResult<T> SQL<T>(string query, params object[] args);
```

## Requirements:
 
The following calls must be unambigous. The calls must select different implementations to process the query for a0, b0, c0 and d0. In addition, a0 and a1 must select the same implementation, as must
b0 and b1, c0 and c1 and d0 and d1. The calls for a2...d2 and a3..d3 may call the generic implementation.

```cs
var a0 = Db.SQL<int>("SELECT p.Age FROM Person p");
var b0 = Db.SQL<string>("SELECT p.Name FROM Person p");
var c0 = Db.SQL<Person>("SELECT p FROM Person p");
var d0 = Db.SQL<bool?>("SELECT p.HasPet FROM Person p");

var a1 = Db.SQL<int>("SELECT p.Age FROM Person p WHERE p.Name = {?}", "Johan");
var b1 = Db.SQL<string>("SELECT p.Name FROM Person p WHERE p.Age = {?}", 46);
var c1 = Db.SQL<Person>("SELECT p FROM Person p WHERE p.Parent != {?}", null);
var d1 = Db.SQL<bool?>("SELECT p.HasPet FROM Person p WHERE p.LastName = {?}", "Lindh");

var a2 = Db.SQL<int>("SELECT p.Age FROM Person p WHERE p.Name = {?} AND p.LastName != {?}", "Johan", null);
var b2 = Db.SQL<string>("SELECT p.Name FROM Person p WHERE p.Age = {?} OR p.Age = {?}", 46, "a lot");
var c2 = Db.SQL<Person>("SELECT p FROM Person p WHERE p.Parent != {?} OR p.Parent.LastName = {?}", null, "Lindh");
var d2 = Db.SQL<bool?>("SELECT p.HasPet FROM Person p WHERE p.LastName = {?} AND p.Name != {?}", "Lindh", "");

var a3 = Db.SQL<int>("SELECT p.Age FROM Person p WHERE p.Name = {?} AND p.LastName != {?} AND p.Age = {?}", "Johan", null, 46);
var b3 = Db.SQL<string>("SELECT p.Name FROM Person p WHERE p.Age = {?} OR p.Age = {?} OR p.Age = {?}", 46, "a lot", null);
var c3 = Db.SQL<Person>("SELECT p FROM Person p WHERE p.Parent != {?} OR p.Parent.LastName = {?} AND p.Age = {?}", null, "Lindh", 0.1);
var d3 = Db.SQL<bool?>("SELECT p.HasPet FROM Person p WHERE p.LastName = {?} AND p.Name != {?} AND p.Age > {?}", "Lindh", "", 0);
```

## Limitations to overcome:

  * C# does not allow overloading on return type only.
  * C# does not allow type restriction for `string`.
  * C# does not allow type restriction for `Nullable<>`.
  * C# does not allow `params` to follow a parameter with default value.
  * C# will select an overload with a default parameter before using `params`.
