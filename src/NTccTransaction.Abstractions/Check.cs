using System;

namespace NTccTransaction.Abstractions
{
    public static class Check
    {
        public static void NotNull(object value, string name)
        {
           if(value == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public static void NotNullOrEmpty(string value, string name)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}
