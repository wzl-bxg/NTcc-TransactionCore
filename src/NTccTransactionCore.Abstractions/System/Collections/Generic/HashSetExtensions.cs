namespace System.Collections.Generic
{
    public static class HashSetExtensions
    {
        public static void AddRange<T>(this HashSet<T> set, ICollection<T> items)
        {
            if(set==null)
            {
                set = new HashSet<T>();
            }

            if(items!=null && items.Count > 0)
            {
                foreach(var item in items)
                {
                    set.Add(item);
                }
            }
        }
    }
}
