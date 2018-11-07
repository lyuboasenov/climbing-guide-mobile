using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Climbing.Guide.Serialization {
   public class JsonSerializer : ISerializer {

      private JsonSerializerSettings JsonSettings { get; set; } = new JsonSerializerSettings {
         ObjectCreationHandling = ObjectCreationHandling.Replace,
         ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
         TypeNameHandling = TypeNameHandling.All,
      };

      private Newtonsoft.Json.JsonSerializer Serializer { get; set; }

      public JsonSerializer(JsonSerializerSettings settings = null) {
         JsonSettings = settings ?? JsonSettings;
         Serializer = Newtonsoft.Json.JsonSerializer.Create(JsonSettings);
      }

      public T Deserialize<T>(Stream stream) {
         using (TextReader textReader = new StreamReader(stream))
         using (JsonReader jsonReader = new JsonTextReader(textReader)) {
            return Serializer.Deserialize<T>(jsonReader);
         }
      }

      public void Serialize<T>(Stream stream, T obj) {
         using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8, 1024, true))
         using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter)) {
            Serializer.Serialize(jsonWriter, obj);

         }
      }
   }
}
