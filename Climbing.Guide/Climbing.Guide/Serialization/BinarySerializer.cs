using Alat.Caching.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Climbing.Guide.Serialization {
   public class BinarySerializer : Serializer {
      private BinaryFormatter Formatter { get; } = new BinaryFormatter();

      public T Deserialize<T>(Stream stream) {
         return (T)Formatter.Deserialize(stream);
      }

      public void Serialize<T>(Stream stream, T obj) {
         Formatter.Serialize(stream, obj);
      }
   }
}
