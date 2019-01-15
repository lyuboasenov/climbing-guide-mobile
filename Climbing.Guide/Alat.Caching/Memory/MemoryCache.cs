using System;
using System.Collections.Generic;
using System.Text;

namespace Alat.Caching.Memory {
   public class MemoryCache : Impl.Cache {
      public MemoryCache(Settings settings, Serialization.Serializer serializer) :
         base(new MemoryCacheStore(settings), serializer) { }
   }
}
