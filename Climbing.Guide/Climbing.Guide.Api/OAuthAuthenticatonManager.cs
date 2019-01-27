using Alat.Patterns.Observer;
using Climbing.Guide.Api.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Climbing.Guide.Api {
   public class OAuthAuthenticatonManager : IAuthenticationManager {
      private IList<IAuthenticationManagerObserver> Observers { get; }

      private HttpClient HttpClient { get; set; }

      public string ClientId { get; set; }
      public string ClientSecret { get; set; }

      protected string Token { get; set; }
      protected string RefreshToken { get; set; }
      protected DateTime TokenExpiration { get; set; }
      

      public string Username { get; protected set; }

      public bool IsLoggedIn {
         get {
            return !string.IsNullOrEmpty(Token);
         }
      }

      public OAuthAuthenticatonManager(HttpClient httpClient, string clientId, string clientSecret) {
         HttpClient = httpClient;
         ClientId = clientId;
         ClientSecret = clientSecret;

         Observers = new List<IAuthenticationManagerObserver>();
      }

      public async Task SetCredentials(HttpRequestMessage request) {
         // Checks if the user have already logged in and if so
         // and the token have expired, the token is refreshed
         if (!string.IsNullOrEmpty(Token) &&
            !string.IsNullOrEmpty(RefreshToken) &&
            DateTime.Now.CompareTo(TokenExpiration) > 0) {
            await RefreshTokenAsync();
         }

         if (!string.IsNullOrEmpty(Token)) {
            request.Headers.Authorization =
               new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
         }
      }

      private async Task RefreshTokenAsync() {
         using (var content = new FormUrlEncodedContent(new Dictionary<string, string>() {
            { "refresh_token", RefreshToken },
            { "grant_type", "refresh_token" },
            { "client_id", ClientId },
            { "client_secret", ClientSecret }
         })) {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "o/token/") {
               Content = content
            }) {
               try {
                  await GetAccessTokenAsync(Username, request);
               } catch (HttpRequestException ex) {
                  throw new TokenRefreshException("Token renewal failed.", ex);
               }
            }
         }
      }

      private async Task GetAccessTokenAsync(string username, HttpRequestMessage requestMessage) {
         using (var response = await HttpClient.SendAsync(requestMessage)) {
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            Dictionary<string, string> authenticationAttributes =
               JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);

            SetCredentials(username,
               authenticationAttributes["access_token"],
               authenticationAttributes["refresh_token"],
               DateTime.Now.AddSeconds(int.Parse(authenticationAttributes["expires_in"])));
         }
      }

      protected virtual void SetCredentials(string username, string token, string refreshToken, DateTime tokenExpiration) {
         Token = token;
         TokenExpiration = tokenExpiration;
         RefreshToken = refreshToken;
      }

      public async Task<bool> LoginAsync(string username, string password) {
         Token = string.Empty;
         RefreshToken = string.Empty;
         Username = string.Empty;
         TokenExpiration = DateTime.MinValue;

         using (var content = new FormUrlEncodedContent(new Dictionary<string, string>() {
            { "username", username },
            { "password", password },
            { "grant_type", "password" }
         })) {
            var authenticationSecret = string.Format("{0}:{1}", ClientId, ClientSecret);
            authenticationSecret = Convert.ToBase64String(Encoding.UTF8.GetBytes(authenticationSecret));

            var result = false;
            using (var request = new HttpRequestMessage(HttpMethod.Post, "o/token/") {
               Content = content
            }) {
               request.Headers.Authorization =
                  new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authenticationSecret);

               try {
                  await GetAccessTokenAsync(username, request);
                  Username = username;
                  result = true;
               } catch (HttpRequestException) { }
            }

            NotifyObserversOnLogIn();
            return result;
         }

         
      }

      public async Task LogoutAsync() {
         var content = new FormUrlEncodedContent(new Dictionary<string, string>() {
            { "username", Token },
            { "client_id", ClientId },
            { "client_secret", ClientSecret }
         });

         using (var response = await HttpClient.PostAsync("o/revoke_token/", content)) {
            Username = string.Empty;
            SetCredentials(string.Empty, string.Empty, string.Empty, DateTime.MinValue);
         }

         NotifyObserversOnLogOut();
      }

      public IDisposable SubscribeObserver(IAuthenticationManagerObserver authenticationManagerObserver) {
         Observers.Add(authenticationManagerObserver);

         return new Unsubscriber<IAuthenticationManagerObserver>(Observers, authenticationManagerObserver);
      }

      private void NotifyObserversOnLogIn() {
         foreach(var observer in Observers) {
            observer.OnLogIn();
         }
      }

      private void NotifyObserversOnLogOut() {
         foreach (var observer in Observers) {
            observer.OnLogOut();
         }
      }
   }
}
