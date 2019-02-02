using System.IO;

namespace Climbing.Guide.Forms.Services.Environment {
   public class Environment : IEnvironment {
      public string ApplicationDataPath { get; }
      public string CachePath { get; }
      public string TempPath { get; }

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
