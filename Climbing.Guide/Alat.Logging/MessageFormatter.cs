namespace Alat.Logging {
   public interface MessageFormatter {
      string Format(string message, Category category, Priority priority);
   }
}
