namespace RubyCase.SaveSystem
{
    public interface ISaveSystem
    {
        bool HasKey(string key);
        bool Save<T>(string key, T data);

        bool TryGet<T>(string key, out T data);
    }
}
