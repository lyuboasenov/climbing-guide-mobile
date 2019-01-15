using Alat.Caching;
using Alat.Caching.Serialization;

namespace Climbing.Guide.Caching.Sqlite {
   public class SqliteCache : Alat.Caching.Impl.Cache {
      public SqliteCache(Settings settings, Serializer serializer) : 
         base(new SqliteCacheStore(settings), serializer) { }
   }
}
