/*
    @author Johan Lindh <johan@linkdata.se>

    Expose a single C# API taking a query string and zero or more optional
    parameters that has a return value that depends on the type parameter,
    and that can select different implementations for different kinds of
    types at compile time.

    You may not use the "dynamic" type. The implementation for class must
    be able to do "new T()".

    The following four sets of type parameters must invoke four different
    implementations:

    * All non-nullable value types (int, char, double...)
    * The nullable string value type (string)
    * All class objects with a default constructor (class)
    * A generic catch-all implementation that can handle Nullable<T> and
      also accepts any number of parameters.

    Essentially, the exposed API looks like this:

      SQLResult<T> SQL<T>(string query, params object[] args);
*/

using System;
using System.Diagnostics;
using System.Reflection;

namespace CSharpFunctionOverloading
{
    public class SQLResult<T>
    {
        public int Params;
        public string Impl;
        public Type Type
        {
            get
            {
                return typeof(T);
            }
        }
    }

    public partial class Db
    {
        // Just here as a template of how the SQL() overloads can look.
        // Kept private so it doesn't interfere with the actual implementation.
        static private SQLResult<T> SQL<T>(string query, params object[] args)
        {
            return HandleGeneric<T>.Check(args is Array ? args.Length : 1);
        }
    }

    public class Person : Db
    {
    }

    public class HandleValue<T>
    {
        static public SQLResult<T> Check(int paramCount)
        {
            Debug.Assert(typeof(T).GetTypeInfo().IsValueType);
            Debug.Assert(typeof(T) != typeof(string));
            return new SQLResult<T>() { Params = paramCount, Impl = "HandleValue" };
        }
    }

    public class HandleString<T>
    {
        static public SQLResult<T> Check(int paramCount)
        {
            Debug.Assert(typeof(T) == typeof(string));
            return new SQLResult<T>() { Params = paramCount, Impl = "HandleString" };
        }
    }

    public class HandleClass<T>
    {
        static public SQLResult<T> Check(int paramCount)
        {
            Debug.Assert(typeof(Db).IsAssignableFrom(typeof(T)));
            return new SQLResult<T>() { Params = paramCount, Impl = "HandleClass" };
        }
    }

    public class HandleGeneric<T>
    {
        static public SQLResult<T> Check(int paramCount)
        {
            return new SQLResult<T>() { Params = paramCount, Impl = "HandleGeneric" };
        }
    }

    public class Program
    {
        public static SQLResult<T> Check<T>(string impl, int paramCount, SQLResult<T> res)
        {
            bool ok = (res.Impl == impl) && (res.Params == paramCount);
            Console.WriteLine("Expecting {0} with {1} parameters: {2}", impl, paramCount, ok ? "PASS" : "FAIL");
            return res;
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Calling with query string and no parameters:");
            var a0 = Check<int>("HandleValue", 0, Db.SQL<int>("SELECT p.Age FROM Person p"));
            var b0 = Check<string>("HandleString", 0, Db.SQL<string>("SELECT p.Name FROM Person p"));
            var c0 = Check<Person>("HandleClass", 0, Db.SQL<Person>("SELECT p FROM Person p"));
            var d0 = Check<bool?>("HandleGeneric", 0, Db.SQL<bool?>("SELECT p.HasPet FROM Person p"));
            Console.WriteLine();

            Console.WriteLine("Calling with query string and 1 parameter:");
            var a1 = Check<int>("HandleValue", 1, Db.SQL<int>("SELECT p.Age FROM Person p WHERE p.Name = {?}", "Johan"));
            var b1 = Check<string>("HandleString", 1, Db.SQL<string>("SELECT p.Name FROM Person p WHERE p.Age = {?}", 46));
            var c1 = Check<Person>("HandleClass", 1, Db.SQL<Person>("SELECT p FROM Person p WHERE p.Parent != {?}", null));
            var d1 = Check<bool?>("HandleGeneric", 1, Db.SQL<bool?>("SELECT p.HasPet FROM Person p WHERE p.LastName = {?}", "Lindh"));
            Console.WriteLine();

            Console.WriteLine("Calling with query string and 2 parameters:");
            var a2 = Check<int>("HandleGeneric", 2, Db.SQL<int>("SELECT p.Age FROM Person p WHERE p.Name = {?} AND p.LastName != {?}", "Johan", null));
            var b2 = Check<string>("HandleGeneric", 2, Db.SQL<string>("SELECT p.Name FROM Person p WHERE p.Age = {?} OR p.Age = {?}", 46, "a lot"));
            var c2 = Check<Person>("HandleGeneric", 2, Db.SQL<Person>("SELECT p FROM Person p WHERE p.Parent != {?} OR p.Parent.LastName = {?}", null, "Lindh"));
            var d2 = Check<bool?>("HandleGeneric", 2, Db.SQL<bool?>("SELECT p.HasPet FROM Person p WHERE p.LastName = {?} AND p.Name != {?}", "Lindh", ""));
            Console.WriteLine();

            Console.WriteLine("Calling with query string and 3 parameters:");
            var a3 = Check<int>("HandleGeneric", 3, Db.SQL<int>("SELECT p.Age FROM Person p WHERE p.Name = {?} ....", "Johan", null, 46));
            var b3 = Check<string>("HandleGeneric", 3, Db.SQL<string>("SELECT p.Name FROM Person p WHERE p.Age = {?} ...", 46, "a lot", null));
            var c3 = Check<Person>("HandleGeneric", 3, Db.SQL<Person>("SELECT p FROM Person p WHERE p.Parent != {?} ...", null, "Lindh", 0.1));
            var d3 = Check<bool?>("HandleGeneric", 3, Db.SQL<bool?>("SELECT p.HasPet FROM Person p WHERE p.LastName = {?} ...", "Lindh", "", 0));
            Console.WriteLine();

            Console.WriteLine("Calling with query string and 4 parameters:");
            var a4 = Check<int>("HandleGeneric", 4, Db.SQL<int>("SELECT p.Age FROM Person p WHERE p.Name = {?} ....", "Johan", null, 46, 4));
            var b4 = Check<string>("HandleGeneric", 4, Db.SQL<string>("SELECT p.Name FROM Person p WHERE p.Age = {?} ...", 46, "a lot", null, "bah"));
            var c4 = Check<Person>("HandleGeneric", 4, Db.SQL<Person>("SELECT p FROM Person p WHERE p.Parent != {?} ...", null, "Lindh", 0.1, null));
            var d4 = Check<bool?>("HandleGeneric", 4, Db.SQL<bool?>("SELECT p.HasPet FROM Person p WHERE p.LastName = {?} ...", "Lindh", "", 0, new object()));
            Console.WriteLine();
        }
    }
}
