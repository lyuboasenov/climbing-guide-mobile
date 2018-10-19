using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

// Register RestAPIClient in the DependencyService
[assembly: Dependency(typeof(Climbing.Guide.Mobile.Common.Services.RestApiClient))]
namespace Climbing.Guide.Mobile.Common.Services {
   public class RestApiClient : Core.API.RestApiClient, IRestApiClient {
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
