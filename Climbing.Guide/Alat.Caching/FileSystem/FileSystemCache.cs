using System;
using System.Collections.Generic;
using System.Text;

namespace Alat.Caching.FileSystem {
   public class FileSystemCache : Impl.Cache {
      public FileSystemCache(Settings settings, Serialization.Serializer serializer) :
         base(new FileSystemCacheStore(settings), serializer) { }
   }
}
