namespace Climbing.Guide.Mobile.Forms.Logging {
   public interface ILoggingService {
      void Log(string message, Category category = Category.Info, Priority priority = Priority.None);
   }
}