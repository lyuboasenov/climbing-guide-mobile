﻿using System;
using System.IO;

namespace Climbing.Guide.Caching {
   public interface ICacheItem {
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
