namespace PFramework
{
    using PFramework.Data;

    public static partial class Peach
    {
        public static T LoadData<T>() where T : DataBase, new()
        {
            T data = new T();
            data.LoadData();
            return data;
        }
    }
}