using Climbing.Guide.Api.Schemas;
using System.Text;

namespace Climbing.Guide.Api.Client {
   public class BaseClient {
      protected void SetSelectedLanguage(StringBuilder urlBuilder) {
         var translatableClient = this as ITranslatableClient;
         if (null != translatableClient &&
            null != translatableClient.SelectedLanguage &&
            !(translatableClient.SelectedLanguage.Default??true)) {
            urlBuilder.Insert(0, $"{translatableClient.SelectedLanguage.Code}/");
         }
      }
   }
}
