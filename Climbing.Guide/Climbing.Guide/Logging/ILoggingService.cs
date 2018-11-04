namespace Climbing.Guide.Logging {
   public interface ILoggingService {
      void Log(string message, Category category = Category.Info, Priority priority = Priority.None);
   }
}