using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Climbing.Guide.Core.API.Schemas;
using Newtonsoft.Json;

namespace Climbing.Guide.Core.API {
   public class RestApiClient {
      private const string clientId = "KoZwAMrSN4XjWC2m0Lkp3gjN9t1h9Vano5avgBWI";
      private const string clientSecret = "FT0YD3LyTr4sZgBKRNUk0vEv8gIinHFbVOIqyd11xQ3zT4GG10NjcffaoPUm3Fw4zfTrCMV0xFxOVtabWWzPYDECFoBhr0ezsLwfl75C6kQC5YMeejEJfbAMr0ZetVKz";

      public static RestApiClient Instance { get; } = new RestApiClient();

      private RestApiClientSettings Settings { get; set; }

      private HttpClient HttpClient { get; set; }  = new HttpClient() { BaseAddress = new Uri("http://127.0.0.1:8000/") };
      private string Token { get; set; }
      private DateTime TokenExpiration { get; set; } = DateTime.MaxValue;
      private string RefreshToken { get; set; }
      private string Username { get; set; }

      private IRegionsClient regionsClient;
      private IAreasClient areasClient;
      private ISectorsClient sectorsClient;
      private IRoutesClient routesClient;
      private IRegisterClient registerClient;

      // Singleton property
      public IRegionsClient RegionsClient {
         get {
            return GetGenericClient(ref regionsClient, (client) => new Client.RegionsClient(client));
         }
      }

      public IAreasClient AreasClient {
         get {
            return GetGenericClient(ref areasClient, (client) => new Client.AreasClient(client));
         }
      }

      public ISectorsClient SectorsClient {
         get {
            return GetGenericClient(ref sectorsClient, (client) => new Client.SectorsClient(client));
         }
      }

      public IRoutesClient RoutesClient {
         get {
            return GetGenericClient(ref routesClient, (client) => new Client.RoutesClient(client));
         }
      }

      public IRegisterClient RegisterClient {
         get {
            return GetGenericClient(ref registerClient, (client) => new Client.RegisterClient(client));
         }
      }

      #region Public

      public static void UpdateRestApiClientSettings(RestApiClientSettings settings) {
         Instance.Settings = settings;
         Instance.HttpClient = new HttpClient() { BaseAddress = new Uri(settings.BaseUrl) };
      }

      public async Task<bool> LoginAsync(string username, string password) {
         var content = new FormUrlEncodedContent(new Dictionary<string,string>() {
            { "username", username },
            { "password", password },
            { "grant_type", "password" }
         });
         Username = username;
         return await GetAccessTokenAsync(content);
      }

      public async Task<bool> RefreshTokenAsync() {
         var httpClient = GetHttpClient();
         var content = new FormUrlEncodedContent(new Dictionary<string, string>() {
            { "refresh_token", RefreshToken },
            { "grant_type", "refresh_token" }
         });
         return await GetAccessTokenAsync(content);
      }

      #endregion Public

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

      private T GetGenericClient<T>(ref T memberVariable, Func<HttpClient, T> factory) {
         if (null == memberVariable) {
            memberVariable = factory(GetHttpClient());
         }

         return memberVariable;
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
