namespace Climbing.Guide.Forms.Services {
   public interface IEnvironment {
      string ApplicationDataPath { get; }
      string CachePath { get; }
      string TempPath { get; }

      string GetTempFileName();
   }
}
