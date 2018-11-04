namespace Climbing.Guide.Forms.Logging {
   public class DebugLoggingService : ILoggingService {

      private Prism.Logging.ILoggerFacade InternalLoggerFacade { get; set; }

      public DebugLoggingService(Prism.Logging.DebugLogger loggerFacade) {
         InternalLoggerFacade = loggerFacade;
      }
      /// <summary>
      /// Write a new log entry with the specified category and priority.
      /// </summary>
      /// <param name="message">Message body to log.</param>
      /// <param name="category">Category of the entry.</param>
      /// <param name="priority">The priority of the entry.</param>
      public void Log(string message, Category category = Category.Info, Priority priority = Priority.None) {
         InternalLoggerFacade.Log(message, (Prism.Logging.Category)category, (Prism.Logging.Priority)priority);
      }
   }
}
