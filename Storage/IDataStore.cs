using System;
using System.Collections.Generic;

namespace EventulaEntranceClient.Storage
{
    public interface IDataStore : IDisposable
    {
        void AddOrUpdate<T>(T data) where T : IStoreObject;

        IEnumerable<T> Load<T>() where T : IStoreObject;
    }
}
