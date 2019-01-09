namespace Climbing.Guide.Forms.Services {
   public interface Environment {
      string ApplicationDataPath { get; }
      string CachePath { get; }
      string TempPath { get; }

      string GetTempFileName();
   }
}
