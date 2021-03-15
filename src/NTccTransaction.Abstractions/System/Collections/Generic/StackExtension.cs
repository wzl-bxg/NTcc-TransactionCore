namespace System.Collections.Generic
{
    public static class StackExtension
    {
        public static bool IsNull<T>(this Stack<T> stack)
        {
            return stack == null ;
        }

        public static bool IsNullOrEmpty<T>(this Stack<T> stack)
        {
            return stack == null || stack.Count <= 0;
        }
    }
}
