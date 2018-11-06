using System.IO;

namespace Climbing.Guide.Serialization {
   public interface ISerializer {
      void Serialize<T>(Stream stream, T obj);
      T Deserialize<T>(Stream stream);
   }
}
