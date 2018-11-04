using System;

namespace Climbing.Guide.Caching {
   public interface ICacheRepository {
      void RemoveAll();
      ICacheItem Get(string key);
      void Remove(string[] key);
      bool Contains(string key);
      void Add(string key, string jsonData, string tag, DateTime dateTime);
      void Refresh(string key, DateTime dateTime);
      void Clean();
      long Count();
   }
}