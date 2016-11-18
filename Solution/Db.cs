using System;
using System.Collections;
using System.Collections.Generic;

namespace CSharpFunctionOverloading
{
    public partial class Db
    {
        static public SQLResult<T> SQL<T>(QueryString<T> query, Invalid __unused = default(Invalid))
        {
            return HandleGeneric<T>.Check(0);
        }

        static public SQLResult<T> SQL<T>(QueryString<T> query, object arg1, Invalid __unused = default(Invalid))
        {
            return HandleGeneric<T>.Check(1);
        }

        static public SQLResult<T> SQL<T>(QueryString<T> query, object arg1, object arg2, params object[] args)
        {
            return HandleGeneric<T>.Check(2 + (args is Array ? args.Length : 1));
        }

        static public SQLResult<T> SQL<T>(QueryStringClass<T> query)
            where T: class, new()
        {
            return HandleClass<T>.Check(0);
        }

        static public SQLResult<T> SQL<T>(QueryStringClass<T> query, object arg1)
            where T: class, new()
        {
            return HandleClass<T>.Check(1);
        }

        static public SQLResult<T> SQL<T>(QueryStringStruct<T> query)
            where T: struct
        {
            return HandleValue<T>.Check(0);
        }

        static public SQLResult<T> SQL<T>(QueryStringStruct<T> query, object arg1)
            where T: struct
        {
            return HandleValue<T>.Check(1);
        }

        static public SQLResult<T> SQL<T>(QueryStringString<T> query)
            where T: class, IEnumerable<char>, IEnumerable, IComparable, IComparable<String>, IConvertible, IEquatable<String>
        {
            return HandleString<T>.Check(0);
        }

        static public SQLResult<T> SQL<T>(QueryStringString<T> query, object arg1)
            where T: class, IEnumerable<char>, IEnumerable, IComparable, IComparable<String>, IConvertible, IEquatable<String>
        {
            return HandleString<T>.Check(1);
        }
    }
}