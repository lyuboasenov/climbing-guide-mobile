using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Climbing.Guide.Mobile.Common.Services {
   public class RestApiClient : Core.API.RestApiClient {
      public new static RestApiClient Instance { get; } = new RestApiClient();

      private RestApiClient() {

      }

      public override async Task<bool> LoginAsync(string username, string password) {
         var result = await base.LoginAsync(username, password);

         var saveSecureStorage = new[] {
            SecureStorage.SetAsync("token", Token),
            SecureStorage.SetAsync("refresh_token", RefreshToken),
            SecureStorage.SetAsync("username", Username)
         };

         await Task.WhenAll(saveSecureStorage);

         return result;
      }

      public override async Task<bool> LogoutAsync() {
         SecureStorage.Remove("token");
         SecureStorage.Remove("refresh_token");
         SecureStorage.Remove("username");

         return await base.LogoutAsync();
      }
   }
}
