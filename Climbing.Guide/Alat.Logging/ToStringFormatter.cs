namespace Alat.Logging {
   public class ToStringFormatter : ObjectFormatter {
      public string Format(object obj) {
         return obj.ToString();
      }
   }
}
