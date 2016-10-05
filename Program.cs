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

        static public IEnumerable<T> SQL<T>(QueryString<T> query, object arg1, object arg2, Invalid __unused = default(Invalid))
        {
            Report<T>("<> o,o,d", query);
            return new List<T>();
        }

        static public IEnumerable<T> SQL<T>(QueryString<T> query, object arg1, object arg2, object arg3, params object[] args)
        {
            Report<T>("<> o,o,o,p", query);
            return new List<T>();
        }

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

        static public IEnumerable<T> SQL<T>(QueryStringClass<T> query, object arg1, object arg2)
            where T: class, new()
        {
            Report<T>("<class, new()> o,o", query);
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

        static public IEnumerable<T> SQL<T>(QueryStringStruct<T> query, object arg1, object arg2)
            where T: struct
        {
            Report<T>("<struct> o,o", query);
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

        static public IEnumerable<T> SQL<T>(QueryStringString<T> query, object arg1, object arg2)
            where T: IComparable<string>
        {
            Report<T>("<IComparable<string>> o o", query);
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
            Console.WriteLine("Calling with no parameters:");
            var x0a = Db.SQL<int>("int");
            var x0b = Db.SQL<string>("string");
            var x0c = Db.SQL<Person>("Person");
            var x0d = Db.SQL<int?>("int?");
            Console.WriteLine();

            Console.WriteLine("Calling with 1 parameter:");
            var x1a = Db.SQL<int>("int", 123);
            var x1b = Db.SQL<string>("string", "hej");
            var x1c = Db.SQL<Person>("Person", new Person());
            var x1d = Db.SQL<int?>("int?", null);
            Console.WriteLine();

            Console.WriteLine("Calling with 2 parameters:");
            var x2a = Db.SQL<int>("int", 123, 456);
            var x2b = Db.SQL<string>("string", "hej", "du");
            var x2c = Db.SQL<Person>("Person", new Person(), null);
            var x2d = Db.SQL<int?>("int?", null, 12);
            Console.WriteLine();

            Console.WriteLine("Calling with 3 parameters:");
            var x3a = Db.SQL<int>("int", 123, 456, 789);
            var x3b = Db.SQL<string>("string", "hej", "du", null);
            var x3c = Db.SQL<Person>("Person", new Person(), null, new Person());
            var x3d = Db.SQL<int?>("int?", null, 12, null);
            Console.WriteLine();

            Console.WriteLine("Calling with 4 parameters:");
            var x4a = Db.SQL<int>("int", 123, 456, 789, null);
            var x4b = Db.SQL<string>("string", "hej", "du", null, "där");
            var x4c = Db.SQL<Person>("Person", new Person(), null, new Person(), 123);
            var x4d = Db.SQL<int?>("int?", null, 12, null, "meh");
            Console.WriteLine();
        }
    }
}
