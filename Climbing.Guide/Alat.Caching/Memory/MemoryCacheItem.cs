using System;
using System.IO;

namespace Alat.Caching.Memory {
   public class MemoryCacheItem : CacheItem {
      public string Key { get; set; }
      public string Tag { get; set; }
      public Stream Content { get; set; }
      public DateTime ExpirationDate { get; set; }
   }
}
