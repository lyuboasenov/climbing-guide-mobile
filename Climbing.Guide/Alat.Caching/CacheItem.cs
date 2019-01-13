using System;
using System.IO;

namespace Alat.Caching {
   public interface CacheItem {
      string Key { get; set; }

      /// <summary>
      /// Additional Tags
      /// </summary>
      string Tag { get; set; }

      /// <summary>
      /// Main Content.
      /// </summary>
      Stream Content { get; set; }

      /// <summary>
      /// Expiration data of the object, stored in UTC
      /// </summary>
      DateTime ExpirationDate { get; set; }
   }
}
