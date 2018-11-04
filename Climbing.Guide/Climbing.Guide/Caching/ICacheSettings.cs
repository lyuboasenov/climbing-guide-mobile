namespace Climbing.Guide.Caching {
   public interface ICacheSettings {
      string Location { get; }
   }

   public class CacheSettings : ICacheSettings {
      public string Location { get; private set; }

      public CacheSettings(string location) { Location = location; }
   }
}
