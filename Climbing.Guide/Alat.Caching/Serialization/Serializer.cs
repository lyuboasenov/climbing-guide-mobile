using System.IO;

namespace Alat.Caching.Serialization {
   public interface Serializer {
      void Serialize<T>(Stream stream, T obj);
      T Deserialize<T>(Stream stream);
   }
}
