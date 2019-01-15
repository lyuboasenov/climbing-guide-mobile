using System;
using System.IO;

namespace Alat.Caching {
   public interface CacheStore {
      bool Contains(string key);

      CacheItem Find(string key);

      void Add(string key, Stream content, string tag, DateTime dateTime);
      void Remove(string[] key);
      void RemoveAll();

      void Reset(string key, DateTime dateTime);

      void Clean();

      bool Any();

      long GetSize();
   }
}