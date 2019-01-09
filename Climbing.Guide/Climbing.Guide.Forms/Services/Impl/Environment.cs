using System.IO;

namespace Climbing.Guide.Forms.Services.Impl {
   public class Environment : Services.Environment {
      public string ApplicationDataPath { get; private set; }
      public string CachePath { get; private set; }
      public string TempPath { get; private set; }

      public Environment() {
         ApplicationDataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
         CachePath = Path.Combine(ApplicationDataPath, "cache");
         TempPath = Path.GetTempPath();

         Directory.Delete(TempPath, true);
         Directory.CreateDirectory(TempPath);
      }

      public string GetTempFileName() {
         return Path.GetTempFileName();
      }
   }
}
