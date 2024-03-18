using System.Collections;

public static class ArrayExtension
{
    public static bool Contains<T>(this T[] array, T target)
    {
        if(array != null)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(target))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
