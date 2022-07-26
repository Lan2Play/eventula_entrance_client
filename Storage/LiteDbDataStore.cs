using LiteDB;

namespace EventulaEntranceClient.Storage;

public class LiteDbDataStore : IDataStore
{
    private readonly LiteDatabase _LiteDatabase;
    private bool _DisposedValue;

    public LiteDbDataStore()
    {
        var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EventulaEntranceClient");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        _LiteDatabase = new LiteDatabase(Path.Combine(folderPath, "LiteDb.db"));
    }

    public void AddOrUpdate<T>(T data)
        where T : IStoreObject
    {
        _LiteDatabase.GetCollection<T>().Upsert(data);
    }

    public IEnumerable<T> Load<T>()
        where T : IStoreObject
    {
        return _LiteDatabase.GetCollection<T>().FindAll().ToList();
    }

    public T LoadById<T>(int id)
        where T : IStoreObject
    {
        return _LiteDatabase.GetCollection<T>().FindById(new BsonValue(id));
    }

    public bool Delete<T>(T data) where T : IStoreObject
    {
        return _LiteDatabase.GetCollection<T>().Delete(new BsonValue(data.Id));
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_DisposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _DisposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~LiteDbDataStore()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        System.GC.SuppressFinalize(this);
    }
}