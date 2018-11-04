using SQLite;
using System;

namespace Climbing.Guide.Caching.Sqlite {
   public class SqliteCacheItem : ICacheItem {
      [PrimaryKey]
      public string Key { get; set; }
      public string Tag { get; set; }
      public string Content { get; set; }
      public DateTime ExpirationDate { get; set; }
   }
}
