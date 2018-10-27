using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Climbing.Guide.Mobile.Forms.Services {
   public class RestApiClient : Core.API.RestApiClient, IRestApiClient {

      public RestApiClient() {

      }

      public RestApiClient(string baseUrl) {
         string token = string.Empty;
         string refreshToken = string.Empty;
         string username = string.Empty;

         try {
            token = SecureStorage.GetAsync("token").GetAwaiter().GetResult();
            refreshToken = SecureStorage.GetAsync("refresh_token").GetAwaiter().GetResult();
            username = SecureStorage.GetAsync("username").GetAwaiter().GetResult();
         } catch (Exception ex) {
            // Possible that device doesn't support secure storage on device.
            Console.WriteLine($"Error: {ex.Message}");
         }

         UpdateRestApiClientSettings(new Core.API.RestApiClientSettings() {
            BaseUrl = baseUrl,
            Username = username,
            Token = token,
            RefreshToken = refreshToken
         });
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

      public async Task<string> DownloadRouteSchemaThumbAsync(int routeId, Uri schemaThumbUri) {
         var localPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), $"climbing-guide/routes/schema/thumb/{routeId}.jpeg");
         await DownloadAsync(schemaThumbUri, localPath);
         return localPath;
      }

      public async Task<string> DownloadRouteSchemaAsync(int routeId, Uri schemaUri) {
         var localPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), $"climbing-guide/routes/schema/full/{routeId}.jpeg");
         await DownloadAsync(schemaUri, localPath);
         return localPath;
      }
   }
}
