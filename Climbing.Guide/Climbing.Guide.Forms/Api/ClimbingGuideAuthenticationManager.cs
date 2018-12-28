using Climbing.Guide.Api;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Climbing.Guide.Forms.Api {
   public class ClimbingGuideAuthenticationManager : OAuthAuthenticatonManager {
      private const string clientId = "vYtYO6T59kiIGcLEnabBO0akWbpXUh9HsgVKwEM9";
      private const string clientSecret = "rPDI7SPeNxK79v8CDZmwiLbVEFtWIc8rkfbVVhuJeJFv8xFrKwSvSHugxhldFI3W1TrWnbqs3rZGintRRKbfIuChc4npwseBI4ntel4wAk8QjwLG68MnKIr19GfnkwCx";

      public ClimbingGuideAuthenticationManager(HttpClient httpClient) : base(httpClient, clientId, clientSecret) {
         try {
            var token = SecureStorage.GetAsync("token").Result;
            var refreshToken = SecureStorage.GetAsync("refresh_token").Result;
            var username = SecureStorage.GetAsync("username").Result;

            if (!string.IsNullOrEmpty(token) &&
               !string.IsNullOrEmpty(refreshToken) &&
               !string.IsNullOrEmpty(username)) {
               Username = username;
               SetCredentials(username, token, refreshToken, DateTime.MinValue);
            }
         } catch (Exception ex) {
            // Possible that device doesn't support secure storage on device.
            Console.WriteLine($"Error: {ex.Message}");
         }
      }

      protected override void SetCredentials(string username, string token, string refreshToken, DateTime tokenExpiration) {
         base.SetCredentials(username, token, refreshToken, tokenExpiration);

         if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshToken)) {
            SecureStorage.Remove("token");
            SecureStorage.Remove("refresh_token");
            SecureStorage.Remove("username");
         } else {
            var saveSecureStorage = new[] {
               SecureStorage.SetAsync("token", Token),
               SecureStorage.SetAsync("refresh_token", RefreshToken),
               SecureStorage.SetAsync("username", username)
            };

            Task.WhenAll(saveSecureStorage);
         }
      }
   }
}
