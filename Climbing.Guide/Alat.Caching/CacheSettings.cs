namespace Alat.Caching { 
   public class CacheSettings {
      public string Location { get; private set; }

      public CacheSettings(string location) { Location = location; }
   }
}
