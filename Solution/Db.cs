using System;

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
            where T: IComparable<string>
        {
            return HandleString<T>.Check(0);
        }

        static public SQLResult<T> SQL<T>(QueryStringString<T> query, object arg1)
            where T: IComparable<string>
        {
            return HandleString<T>.Check(1);
        }
    }
}