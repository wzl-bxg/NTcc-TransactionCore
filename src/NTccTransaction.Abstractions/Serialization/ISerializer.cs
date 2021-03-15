namespace NTccTransaction.Abstractions
{
    public interface ISerializer
    {
        T Deserialize<T>(string content);

        string Serialize<T>(T obj);
    }
}
