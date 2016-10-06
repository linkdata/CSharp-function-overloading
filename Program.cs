//
// @author Johan Lindh <johan@linkdata.se>
//

using System;
using System.Reflection;
using System.Collections.Generic;

namespace CSharpFunctionOverloading
{
    public struct Invalid
    {
        void InvalidInterfaceHasNoMethods()
        {
        }
    }

    public class Db
    {
        static public void Report<T>(string variant, string query, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            TypeInfo ti = typeof(T).GetTypeInfo();
            Console.WriteLine("{0}<{2}>(\"{3}\") {1} {4}",
                memberName, variant, typeof(T), query,
                (ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(Nullable<>)) ? " Nullable": ""
                );
        }

        // Slow fallback SQL() that does runtime inspection of T
        static public IEnumerable<T> SQL<T>(QueryString<T> query, Invalid __unused = default(Invalid))
        {
            Report<T>("<> d", query);
            return new List<T>();
        }

        static public IEnumerable<T> SQL<T>(QueryString<T> query, object arg1, Invalid __unused = default(Invalid))
        {
            Report<T>("<> o,d", query);
            return new List<T>();
        }

        static public IEnumerable<T> SQL<T>(QueryString<T> query, object arg1, object arg2, params object[] args)
        {
            Report<T>("<> o,o,p", query);
            return new List<T>();
        }

        // 
        static public IEnumerable<T> SQL<T>(QueryStringClass<T> query)
            where T: class, new()
        {
            Report<T>("<class, new()>", query);
            return new List<T>();
        }

        static public IEnumerable<T> SQL<T>(QueryStringClass<T> query, object arg1)
            where T: class, new()
        {
            Report<T>("<class, new()> o", query);
            return new List<T>();
        }

        static public IEnumerable<T> SQL<T>(QueryStringStruct<T> query)
            where T: struct
        {
            Report<T>("<struct>", query);
            return new List<T>();
        }

        static public IEnumerable<T> SQL<T>(QueryStringStruct<T> query, object arg1)
            where T: struct
        {
            Report<T>("<struct> o", query);
            return new List<T>();
        }

        static public IEnumerable<T> SQL<T>(QueryStringString<T> query)
            where T: IComparable<string>
        {
            Report<T>("<IComparable<string>>", query);
            return new List<T>();
        }

        static public IEnumerable<T> SQL<T>(QueryStringString<T> query, object arg1)
            where T: IComparable<string>
        {
            Report<T>("<IComparable<string>> o", query);
            return new List<T>();
        }
    }

    public class Person : Db
    {
    }   

    public class Program
    {
        public static void Main(string[] args)
        {
            //
            // Problem:
            //
            // Expose a single API taking a query string and zero or more optional parameters
            // that has a return value that depends on the type parameter, and that can
            // select different implementations for different kinds of types at compile time.
            //
            // The following type parameters must invoke different implementations:
            //  * All non-nullable value types (where T: struct)
            //  * The nullable string value type (where T: string)
            //  * All class objects (where T: class, new())
            //  * A generic catch-all implementation that can handle Nullable<T>
            //    and also accepts any number of parameters.
            //
            // Essentially, the exposed API looks like this:
            //
            //   SQLResult<T> SQL<T>(string query, params object[] args);
            //
            // The following calls must be unambigous. The calls must select
            // different implementations to process the query for a0, b0, c0 and d0.
            // In addition, a0 and a1 must select the same implementation, as must
            // b0 and b1, c0 and c1 and d0 and d1. The calls for a2...d2 and a3..d3 
            // may call the generic implementation.
            //
            //   var a0 = Db.SQL<int>("SELECT p.Age FROM Person p");
            //   var b0 = Db.SQL<string>("SELECT p.Name FROM Person p");
            //   var c0 = Db.SQL<Person>("SELECT p FROM Person p");
            //   var d0 = Db.SQL<bool?>("SELECT p.HasPet FROM Person p");
            //
            //   var a1 = Db.SQL<int>("SELECT p.Age FROM Person p WHERE p.Name = {?}", "Johan");
            //   var b1 = Db.SQL<string>("SELECT p.Name FROM Person p WHERE p.Age = {?}", 46);
            //   var c1 = Db.SQL<Person>("SELECT p FROM Person p WHERE p.Parent != {?}", null);
            //   var d1 = Db.SQL<bool?>("SELECT p.HasPet FROM Person p WHERE p.LastName = {?}", "Lindh");
            //
            //   var a2 = Db.SQL<int>("SELECT p.Age FROM Person p WHERE p.Name = {?} AND p.LastName != {?}", "Johan", null);
            //   var b2 = Db.SQL<string>("SELECT p.Name FROM Person p WHERE p.Age = {?} OR p.Age = {?}", 46, "a lot");
            //   var c2 = Db.SQL<Person>("SELECT p FROM Person p WHERE p.Parent != {?} OR p.Parent.LastName = {?}", null, "Lindh");
            //   var d2 = Db.SQL<bool?>("SELECT p.HasPet FROM Person p WHERE p.LastName = {?} AND p.Name != {?}", "Lindh", "");
            //
            //   var a3 = Db.SQL<int>("SELECT p.Age FROM Person p WHERE p.Name = {?} AND p.LastName != {?} AND p.Age = {?}", "Johan", null, 46);
            //   var b3 = Db.SQL<string>("SELECT p.Name FROM Person p WHERE p.Age = {?} OR p.Age = {?} OR p.Age = {?}", 46, "a lot", null);
            //   var c3 = Db.SQL<Person>("SELECT p FROM Person p WHERE p.Parent != {?} OR p.Parent.LastName = {?} AND p.Age = {?}", null, "Lindh", 0.1);
            //   var d3 = Db.SQL<bool?>("SELECT p.HasPet FROM Person p WHERE p.LastName = {?} AND p.Name != {?} AND p.Age > {?}", "Lindh", "", 0);
            //
            // Limitations to overcome:
            //
            //   * C# does not allow overloading on return type only.
            //   * C# does not allow type restriction for "string".
            //   * C# does not allow type restriction for "Nullable<>".
            //   * C# does not allow "params" to follow a parameter with default value.
            //   * C# will select an overload with a default parameter before using "params".

            Console.WriteLine("Calling with query string and no parameters:");
            var a0 = Db.SQL<int>("SELECT p.Age FROM Person p");
            var b0 = Db.SQL<string>("SELECT p.Name FROM Person p");
            var c0 = Db.SQL<Person>("SELECT p FROM Person p");
            var d0 = Db.SQL<bool?>("SELECT p.HasPet FROM Person p");
            Console.WriteLine();

            Console.WriteLine("Calling with query string and 1 parameter:");
            var a1 = Db.SQL<int>("SELECT p.Age FROM Person p WHERE p.Name = {?}", "Johan");
            var b1 = Db.SQL<string>("SELECT p.Name FROM Person p WHERE p.Age = {?}", 46);
            var c1 = Db.SQL<Person>("SELECT p FROM Person p WHERE p.Parent != {?}", null);
            var d1 = Db.SQL<bool?>("SELECT p.HasPet FROM Person p WHERE p.LastName = {?}", "Lindh");
            Console.WriteLine();

            Console.WriteLine("Calling with query string and 2 parameters:");
            var a2 = Db.SQL<int>("SELECT p.Age FROM Person p WHERE p.Name = {?} AND p.LastName != {?}", "Johan", null);
            var b2 = Db.SQL<string>("SELECT p.Name FROM Person p WHERE p.Age = {?} OR p.Age = {?}", 46, "a lot");
            var c2 = Db.SQL<Person>("SELECT p FROM Person p WHERE p.Parent != {?} OR p.Parent.LastName = {?}", null, "Lindh");
            var d2 = Db.SQL<bool?>("SELECT p.HasPet FROM Person p WHERE p.LastName = {?} AND p.Name != {?}", "Lindh", "");
            Console.WriteLine();

            Console.WriteLine("Calling with query string and 3 or more parameters:");
            var a3 = Db.SQL<int>("SELECT p.Age FROM Person p WHERE p.Name = {?} ....", "Johan", null, 46);
            var b3 = Db.SQL<string>("SELECT p.Name FROM Person p WHERE p.Age = {?} ...", 46, "a lot", null);
            var c3 = Db.SQL<Person>("SELECT p FROM Person p WHERE p.Parent != {?} ...", null, "Lindh", 0.1);
            var d3 = Db.SQL<bool?>("SELECT p.HasPet FROM Person p WHERE p.LastName = {?} ...", "Lindh", "", 0);
            Console.WriteLine();
        }
    }
}
