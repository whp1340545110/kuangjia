namespace System.Collections.Generic
{
    using System.Text;

    public static class CollectionExtensions
    {
        public static T Last<T>(this IList<T> list)
        {
            if(list.Count > 0)
            {
                return list[list.Count - 1];
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// ForEach的方式迭代List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="itemAction"></param>
        /// <param name="onException"></param>
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> itemAction, Action<T,Exception> onException)
        {
            if(list != null)
            {
                foreach(var item in list)
                {
                    try
                    {
                        itemAction?.Invoke(item);
                    }
                    catch(Exception e)
                    {
                        onException?.Invoke(item, e);
                    }
                }
            }
        }

        /// <summary>
        /// for循环迭代List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="itemAction"></param>
        /// <param name="onException"></param>
        public static void ForIndex<T>(this IList<T> list, Action<int,T> itemAction, Action<int, Exception> onException = null)
        {
            if (list != null)
            {
                int count = list.Count;
                for(int i = 0; i < count; i++)
                {
                    try
                    {
                        itemAction?.Invoke(i,list[i]);
                    }
                    catch (Exception e)
                    {
                        onException?.Invoke(i, e);
                    }
                }
            }
        }

        /// <summary>
        /// 查找下标
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="match"></param>
        /// <param name="onException"></param>
        public static int IndexOf<T>(this IList<T> list, Func<T, bool> match, Action<int, Exception> onException = null)
        {
            if (list != null)
            {
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        if(match.Invoke(list[i]))
                        {
                            return i;
                        };
                    }
                    catch (Exception e)
                    {
                        onException?.Invoke(i, e);
                        return -1;
                    }
                }
            }
            return -1;
        }

        public static TValue GetOrCreate<TKey,TValue>(this IDictionary<TKey,TValue> dic, TKey key, Func<TValue> createValueFunc)
        {
            if (!dic.TryGetValue(key, out TValue value))
            {
                value = createValueFunc();
                dic.Add(key, value);
            }
            return value;
        }

        public static string ForEachToString<T>(this IList<T> list, string seperate = ";")
        {
            if (list.Count == 0)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            int count = list.Count;
            for(int i = 0; i < count; i++)
            {
                sb.Append(list[i]).ToString();
                sb.Append(seperate);
            }

            return sb.Remove(sb.Length - 1, 1).ToString();
        }

        public static bool Contains<T>(this IEnumerable<T> ts,T item)
        {
            foreach(var t in ts)
            {
                if (t.Equals(item))
                {
                    return true;
                }
            }
            return false;
        }


        public static T Find<T>(this IEnumerable<T> ts, System.Func<T,bool> func)
        {
            foreach (var t in ts)
            {
                if (func(t))
                {
                    return t;
                }
            }
            return default(T);
        }

        public static ICollection<T> FindAll<T>(this IEnumerable<T> ts, System.Func<T, bool> func)
        {
            List<T> list = new List<T>();
            foreach (var t in ts)
            {
                if (func(t))
                {
                    list.Add(t);
                }
            }
            return list;
        }
    }
}
