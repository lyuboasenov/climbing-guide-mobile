using System.Text;
using Climbing.Guide.Api.Schemas;

namespace Climbing.Guide.Api.Client {
   public partial class RoutesClient : ITranslatableClient {
      public Language SelectedLanguage { get; set; }

      partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, StringBuilder urlBuilder) {
         SetSelectedLanguage(urlBuilder);
      }
   }
}
