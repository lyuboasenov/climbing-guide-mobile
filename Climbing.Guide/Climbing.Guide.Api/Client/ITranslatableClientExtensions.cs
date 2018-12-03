using System;
using System.Text;

namespace Climbing.Guide.Api.Client {
   public static class ITranslatableClientExtensions {
      public static void SetSelectedLanguage(this ITranslatableClient self, StringBuilder urlBuilder) {
         if (null == self) {
            throw new ArgumentNullException(nameof(self));
         }

         if (null != self.SelectedLanguage &&
            !(self.SelectedLanguage.Default ?? true)) {
            urlBuilder.Insert(0, $"{self.SelectedLanguage.Code}/");
         }
      }
   }
}
