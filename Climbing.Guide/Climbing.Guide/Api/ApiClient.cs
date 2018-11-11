using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Climbing.Guide.Api.Client;
using Climbing.Guide.Api.Schemas;
using Newtonsoft.Json;

namespace Climbing.Guide.Core.Api {
   public class ApiClient : IApiClient {

      private IApiClientSettings Settings { get; set; }

      private const string clientId = "KoZwAMrSN4XjWC2m0Lkp3gjN9t1h9Vano5avgBWI";
      private const string clientSecret = "FT0YD3LyTr4sZgBKRNUk0vEv8gIinHFbVOIqyd11xQ3zT4GG10NjcffaoPUm3Fw4zfTrCMV0xFxOVtabWWzPYDECFoBhr0ezsLwfl75C6kQC5YMeejEJfbAMr0ZetVKz";

      private HttpClient HttpClient { get; set; }  = new HttpClient() { BaseAddress = new Uri("http://127.0.0.1:8000/") };
      public string Token { get; private set; }
      private DateTime TokenExpiration { get; set; } = DateTime.MaxValue;
      public string RefreshToken { get; private set; }
      public string Username { get; private set; }

      private object apiClientsLock = new object();
      private IDictionary<Type, BaseClient> ApiClients { get; } = new Dictionary<Type, BaseClient>();

      public ApiClient() {

      }

      public ApiClient(IApiClientSettings settings) {
         UpdateApiClientSettings(settings);
      }

      // Singleton property
      public IRegionsClient RegionsClient {
         get {
            return GetGenericClient((client) => new RegionsClient(client));
         }
      }

      public IAreasClient AreasClient {
         get {
            return GetGenericClient((client) => new AreasClient(client));
         }
      }

      public ISectorsClient SectorsClient {
         get {
            return GetGenericClient((client) => new SectorsClient(client));
         }
      }

      public IRoutesClient RoutesClient {
         get {
            return GetGenericClient((client) => new RoutesClient(client));
         }
      }

      public IUsersClient UsersClient {
         get {
            return GetGenericClient((client) => new UsersClient(client));
         }
      }

      public IGradesClient GradesClient {
         get {
            return GetGenericClient((client) => new GradesClient(client));
         }
      }

      public ILanguagesClient LanguagesClient {
         get {
            return GetGenericClient((client) => new LanguagesClient(client));
         }
      }

      public bool IsLoggedIn {
         get {
            return !string.IsNullOrEmpty(Token);
         }
      }
      
      #region Public

      public void UpdateApiClientSettings(IApiClientSettings settings) {
         Settings = settings;
         Username = settings.Username;
         Token = settings.Token;
         RefreshToken = settings.RefreshToken;
         HttpClient = settings.HttpClient;
         ApiClients.Clear();
      }

      public virtual async Task<bool> LoginAsync(string username, string password) {
         var content = new FormUrlEncodedContent(new Dictionary<string,string>() {
            { "username", username },
            { "password", password },
            { "grant_type", "password" }
         });
         Username = username;
         return await GetAccessTokenAsync(content);
      }

      public virtual async Task<bool> LogoutAsync() {
         var content = new FormUrlEncodedContent(new Dictionary<string, string>() {
            { "username", Token },
            { "client_id", clientId },
            { "client_secret", clientSecret }
         });

         var httpClient = GetHttpClient();

         var response = await httpClient.PostAsync("o/revoke_token/", content);

         Token = string.Empty;
         RefreshToken = string.Empty;
         Username = string.Empty;
         TokenExpiration = DateTime.MaxValue;

         return response.IsSuccessStatusCode;
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

      private async Task<bool> RefreshTokenAsync() {
         var httpClient = GetHttpClient();
         var content = new FormUrlEncodedContent(new Dictionary<string, string>() {
            { "refresh_token", RefreshToken },
            { "grant_type", "refresh_token" }
         });
         return await GetAccessTokenAsync(content);
      }

      private async Task<bool> GetAccessTokenAsync(FormUrlEncodedContent content) {
         var httpClient = GetHttpClient();
         AddClientCredentials(httpClient);

         var response = await httpClient.PostAsync("o/token/", content);
         if (response.IsSuccessStatusCode) {
            var responseContent = await response.Content.ReadAsStringAsync();
            Dictionary<string, string> authenticationAttributes =
               JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);

            Token = authenticationAttributes["access_token"];
            TokenExpiration = DateTime.Now.AddSeconds(int.Parse(authenticationAttributes["expires_in"]));
            RefreshToken = authenticationAttributes["refresh_token"];
         } else {
            Token = string.Empty;
            TokenExpiration = DateTime.MaxValue;
            RefreshToken = string.Empty;
            Username = string.Empty;
         }

         return response.IsSuccessStatusCode;
      }

      private void AddClientCredentials(HttpClient content) {
         var authenticationSecret = string.Format("{0}:{1}", clientId, clientSecret);
         authenticationSecret = Convert.ToBase64String(Encoding.UTF8.GetBytes(authenticationSecret));
         content.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authenticationSecret);
      }

      private T GetGenericClient<T>(Func<HttpClient, T> factory) where T : BaseClient {

         lock (apiClientsLock) {
            if (!ApiClients.ContainsKey(typeof(T))) {
               ApiClients.Add(typeof(T), factory(GetHttpClient()));
            }
         }

         return (T)ApiClients[typeof(T)];
      }

      private HttpClient GetHttpClient() {
         // Checks if the user have already logged in and if so
         // and the token have expired, the token is refreshed
         if(!string.IsNullOrEmpty(Token) && !string.IsNullOrEmpty(RefreshToken) && DateTime.Now.CompareTo(TokenExpiration) > 0) {
            RefreshTokenAsync().RunSynchronously();
         }

         if (!string.IsNullOrEmpty(Token)) {
            HttpClient.DefaultRequestHeaders.Authorization =
               new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
         } else {
            HttpClient.DefaultRequestHeaders.Remove("Authentication");
         }

         return HttpClient;
      }
   }
}
