using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Climbing.Guide.Api {
   public class OAuthHandler : DelegatingHandler {
      private IAuthenticationManager AuthenticationManager { get; }

      public OAuthHandler(IAuthenticationManager authenticationManager) : 
         this (authenticationManager, new HttpClientHandler()) {
      }

      public OAuthHandler(IAuthenticationManager authenticationManager, HttpMessageHandler innerHandler) : base(innerHandler) {
         AuthenticationManager = authenticationManager?? throw new ArgumentNullException(nameof(authenticationManager)); ;
      }

      protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
         AuthenticationManager.SetCredentials(request);
         return base.SendAsync(request, cancellationToken);
      }
   }
}
