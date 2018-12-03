using System.Net.Http;
using System.Text;
using Climbing.Guide.Api.Schemas;

namespace Climbing.Guide.Api.Client {
   public partial class RegionsClient : ITranslatableClient {
      public Language SelectedLanguage { get; set; }

      partial void PrepareRequest(HttpClient client, HttpRequestMessage request, StringBuilder urlBuilder) {
         base.PrepareRequest(client, request, urlBuilder);
         this.SetSelectedLanguage(urlBuilder);
      }

      partial void ProcessResponse(HttpClient client, HttpResponseMessage response) {
         base.ProcessResponse(client, response);
      }
   }
}
