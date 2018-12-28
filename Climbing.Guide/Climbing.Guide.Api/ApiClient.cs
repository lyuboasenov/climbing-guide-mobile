using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Climbing.Guide.Api;
using Climbing.Guide.Api.Client;
using Climbing.Guide.Api.Schemas;

namespace Climbing.Guide.Core.Api {
   public class ApiClient : IApiClient {

      private IApiClientSettings Settings { get; set; }

      private HttpClient HttpClient { get; set; }  = new HttpClient() { BaseAddress = new Uri("http://127.0.0.1:8000/") };
      private ServiceClientSettings ServiceClientSettings { get; set; }
      public IAuthenticationManager AuthenticationManager { get; private set; }

      private readonly object apiClientsLock = new object();
      private IDictionary<Type, ServiceClient> ServiceClients { get; } = new Dictionary<Type, ServiceClient>();

      public ApiClient() {

      }

      public ApiClient(IApiClientSettings settings) {
         UpdateApiClientSettings(settings);
      }

      #region Public

      public IAreasClient AreasClient {
         get {
            return GetGenericClient((settings, client) => new AreasClient(settings, client));
         }
      }

      public IRoutesClient RoutesClient {
         get {
            return GetGenericClient((settings, client) => new RoutesClient(settings, client));
         }
      }

      public IUsersClient UsersClient {
         get {
            return GetGenericClient((settings, client) => new UsersClient(settings, client));
         }
      }

      public IGradesClient GradesClient {
         get {
            return GetGenericClient((settings, client) => new GradesClient(settings, client));
         }
      }

      public ILanguagesClient LanguagesClient {
         get {
            return GetGenericClient((settings, client) => new LanguagesClient(settings, client));
         }
      }

      public void UpdateApiClientSettings(IApiClientSettings settings) {
         Settings = settings;
         AuthenticationManager = settings.AuthenticationManager;
         HttpClient = settings.HttpClient;
         ServiceClients.Clear();
         ServiceClientSettings = new ServiceClientSettings(AuthenticationManager);
      }

      public async Task DownloadAsync(Uri uri, string localPath, bool overwrite = false) {
         var localFile = new FileInfo(localPath);
         if (!localFile.Directory.Exists) {
            localFile.Directory.Create();
         }

         if (!localFile.Exists || overwrite) {
            using (var response = await HttpClient.GetAsync(uri)) {
               response.EnsureSuccessStatusCode();

               using (var localFileStream = localFile.Create()) {
                  using (var responseStream = await response.Content.ReadAsStreamAsync()) {
                     await responseStream.CopyToAsync(localFileStream);
                  }
               }
            }
         }
      }

      #endregion Public

      private T GetGenericClient<T>(Func<ServiceClientSettings, HttpClient, T> factory) where T : ServiceClient {

         lock (apiClientsLock) {
            if (!ServiceClients.ContainsKey(typeof(T))) {
               ServiceClients.Add(typeof(T), factory(ServiceClientSettings, HttpClient));
            }
         }

         return (T)ServiceClients[typeof(T)];
      }
   }
}
