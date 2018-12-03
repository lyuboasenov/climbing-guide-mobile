namespace Climbing.Guide.Api.Client {
   public class ServiceClientSettings : IServiceClientSettings {
      public IAuthenticationManager AuthenticationManager { get; private set; }

      public ServiceClientSettings(IAuthenticationManager credentialManager) {
         AuthenticationManager = credentialManager;
      }
   }
}
