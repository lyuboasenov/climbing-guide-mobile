using System;
using System.Text;

namespace Climbing.Guide.Extensions {
   public static class ExceptionExt {
      public static string ToLogMessage(this Exception self) {
         StringBuilder sb = new StringBuilder();
         Exception ex = self;
         string indent = string.Empty;
         sb.AppendLine($"====================================================== ERROR: ======================================================");
         while (ex != null) {
            sb.AppendLine($"{indent}Type: {ex.GetType()}");
            sb.AppendLine($"{indent}Message: {ex.Message}");
            sb.AppendLine($"{indent}Message: {ex.StackTrace.Replace(Environment.NewLine, Environment.NewLine + indent)}");
            sb.AppendLine($"--------------------------------------------------------------------------------------------------------------------");

            indent += "   ";
            ex = ex.InnerException;
         }
         sb.AppendLine($"=====================================================================================================================");

         return sb.ToString();
      }
   }
}
