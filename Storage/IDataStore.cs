
namespace EventulaEntranceClient.Storage;

public interface IDataStore : IDisposable
{
    void AddOrUpdate<T>(T data) where T : IStoreObject;

    IEnumerable<T> Load<T>() where T : IStoreObject;

    T LoadById<T>(int id) where T : IStoreObject;

    bool Delete<T>(T data) where T : IStoreObject;
}
