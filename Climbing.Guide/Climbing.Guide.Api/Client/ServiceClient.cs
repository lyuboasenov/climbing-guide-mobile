using System;
using System.Net.Http;
using System.Text;

namespace Climbing.Guide.Api.Client {
   public class ServiceClient {

      private IServiceClientSettings Settings { get; set; }

      public ServiceClient(IServiceClientSettings settings) {
         Settings = settings ?? throw new ArgumentNullException(nameof(settings));
      }

      protected virtual void PrepareRequest(HttpClient client, HttpRequestMessage request, StringBuilder urlBuilder) {

      }

      protected virtual void ProcessResponse(HttpClient client, HttpResponseMessage response) {
      }
   }
}
