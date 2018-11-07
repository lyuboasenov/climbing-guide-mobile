using Climbing.Guide.Core.Api;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Climbing.Guide.Forms.Services {
   public class ApiClient : Core.Api.ApiClient, IApiClient {
      public ApiClient(IApiClientSettings settings) {
         try {
            settings.Token = SecureStorage.GetAsync("token").GetAwaiter().GetResult();
            settings.RefreshToken = SecureStorage.GetAsync("refresh_token").GetAwaiter().GetResult();
            settings.Username = SecureStorage.GetAsync("username").GetAwaiter().GetResult();
         } catch (Exception ex) {
            // Possible that device doesn't support secure storage on device.
            Console.WriteLine($"Error: {ex.Message}");
         }

         UpdateRestApiClientSettings(settings);
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
