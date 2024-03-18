namespace PFramework
{
    using System;
    public class Arguments
    {
        internal object[] args;

        public T Get<T>(int argIndex)
        {
            T result = default(T);
            if (args != null && args.Length > argIndex)
            {
                try
                {
                    var resultType = typeof(T);
                    if (resultType.IsSubclassOf(typeof(ValueType)))
                    {
                        if (resultType.IsSubclassOf(typeof(Enum)))
                        {
                            result = (T)Enum.ToObject(resultType, args[argIndex]);
                        }
                        else
                        {
                            result = (T)Convert.ChangeType(args[argIndex], resultType);
                        }
                    }
                    else
                    {
                        result = (T)args[argIndex];
                    }
                }
                catch (Exception e)
                {
                    PDebug.LogWarning($"Conver Error:{e}");
                }
            }
            return result;
        }

        public int Count => args.Length;

        public Arguments() { }
        public Arguments(object[] objects)
        {
            args = objects;
        }
    }
}