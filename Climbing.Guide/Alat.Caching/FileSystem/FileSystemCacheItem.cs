using Alat.Caching;
using System;
using System.IO;

namespace Climbing.Guide.Caching.FileSystem {
   public class FileSystemCacheItem : CacheItem {
      public string Key { get; set; }
      public string Tag { get; set; }
      public Stream Content { get; set; }
      public DateTime ExpirationDate { get; set; }
   }
}
