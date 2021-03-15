namespace NTccTransaction.Abstractions
{
    public enum Propagation:int
    {
        REQUIRED = 0,

        SUPPORTS = 1,

        MANDATORY = 2,

        REQUIRES_NEW = 3
    }
}
