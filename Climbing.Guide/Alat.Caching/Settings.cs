using Alat.Caching.Serialization;

namespace Alat.Caching { 
   public sealed class Settings {
      public string Location { get; }
      public Serializer Serializer { get; }

      public Settings(string location, Serializer serializer) {
         Location = location;
         Serializer = serializer;
      }
   }
}
