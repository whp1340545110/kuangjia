namespace UniRx
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public static class RxCollectionExtensions
    {
        public static T Find<T>(this IReactiveCollection<T> collection, Func<T, bool> onAction, Action<T,Exception> onException = null)
        {
            T result = default(T);
            if (onAction == null)
            {
                if (collection.Count > 0)
                {
                    result = collection[0];
                }
            }
            else
            {
                foreach (T temp in collection)
                {
                    try
                    {
                        if (onAction(temp))
                        {
                            result = temp;
                            break;
                        }
                    }
                    catch(Exception e)
                    {
                        onException?.Invoke(temp, e);
                    }
                }
            }
            return result;
        }

        public static List<T> FindAll<T>(this ReactiveCollection<T> collection, Func<T, bool> onAction, Action<T,Exception> onException = null)
        {
            List<T> results = new List<T>();
            if (onAction == null)
            {
                results.AddRange(collection);
            }
            else
            {
                foreach (T temp in collection)
                {
                    try
                    {
                        if (onAction(temp))
                        {
                            results.Add(temp);
                        }
                    }
                    catch(Exception e)
                    {
                        onException?.Invoke(temp,e);
                    }
                }
            }
            return results;
        }

        public static void AddRange<T>(this ReactiveCollection<T> collection, IEnumerable<T> addtion)
        {
            foreach(T temp in addtion)
            {
                collection.Add(temp);
            }
        }

    }
}
