namespace NTccTransactionCore.Abstractions
{
    public enum MethodRole : int
    {
        ROOT = 0,
        CONSUMER = 1,
        PROVIDER = 2,
        NORMAL = 3
    }
}
