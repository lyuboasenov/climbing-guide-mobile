using System;
using System.IO;

namespace Climbing.Guide.Caching {
   public interface ICacheRepository {
      void RemoveAll();
      ICacheItem Get(string key);
      void Remove(string[] key);
      bool Contains(string key);
      void Add(string key, Stream content, string tag, DateTime dateTime);
      void Refresh(string key, DateTime dateTime);
      void Clean();
      long Count();
      long GetSize();
   }
}