namespace Climbing.Guide.Forms.Services.Environment {
   public interface IEnvironment {
      string ApplicationDataPath { get; }
      string CachePath { get; }
      string TempPath { get; }

      string GetTempFileName();
   }
}
