using System;

namespace CSharpFunctionOverloading
{
    public struct QueryString<T>
    {
        public readonly string Query;
        public QueryString(string query)
        {
            Query = query;
        }
        static public implicit operator QueryString<T>(string query)
        {
            return new QueryString<T>(query);
        }
        static public implicit operator string(QueryString<T> obj)
        {
            return obj.Query;
        }
    }

    public struct QueryStringClass<T>
        where T: class, new()
    {
        public readonly string Query;
        public QueryStringClass(string query)
        {
            Query = query;
        }
        static public implicit operator QueryStringClass<T>(string query)
        {
            return new QueryStringClass<T>(query);
        }
        static public implicit operator string(QueryStringClass<T> obj)
        {
            return obj.Query;
        }
    }

    public struct QueryStringString<T>
        where T: IComparable<string>
    {
        public readonly string Query;
        public QueryStringString(string query)
        {
            Query = query;
        }
        static public implicit operator QueryStringString<T>(string query)
        {
            return new QueryStringString<T>(query);
        }
        static public implicit operator string(QueryStringString<T> obj)
        {
            return obj.Query;
        }
    }

    public struct QueryStringStruct<T>
        where T: struct
    {
        public readonly string Query;
        public QueryStringStruct(string query)
        {
            Query = query;
        }
        static public implicit operator QueryStringStruct<T>(string query)
        {
            return new QueryStringStruct<T>(query);
        }
        static public implicit operator string(QueryStringStruct<T> obj)
        {
            return obj.Query;
        }
    }
}
