using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EventulaEntranceClient.Storage
{
    public class LiteDbDataStore : IDataStore
    {
        private readonly LiteDatabase _LiteDatabase;
        private bool _DisposedValue;

        public LiteDbDataStore()
        {
            _LiteDatabase = new LiteDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EventulaEntranceClient", "LiteDb.db"));
        }

        public void AddOrUpdate<T>(T data)
            where T : IStoreObject
        {
            _LiteDatabase.GetCollection<T>().Upsert(new BsonValue(data.Id), data);
            throw new System.NotImplementedException();
        }

        public IEnumerable<T> Load<T>()
            where T : IStoreObject
        {
            return _LiteDatabase.GetCollection<T>().FindAll().ToList();
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
}
