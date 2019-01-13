using Climbing.Guide.Tasks;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services.Navigation {
   public interface Navigation {
      //
      // Summary:
      //     Navigates to the most recent entry in the back navigation history by popping
      //     the calling Page off the navigation stack.
      //
      // Returns:
      //     If true a go back operation was successful. If false the go back operation failed.
      Task GoBackAsync();
      //
      // Summary:
      //     Navigates to the most recent entry in the back navigation history by popping
      //     the calling Page off the navigation stack.
      //
      // Parameters:
      //   parameters:
      //     The navigation parameters
      //
      // Returns:
      //     If true a go back operation was successful. If false the go back operation failed.
      Task NavigateAsync(NavigationRequest request);

      NavigationRequest GetNavigationRequest(string path, NavigationRequest baseRequest = null);

      NavigationRequest GetNavigationRequest<TParameters>(string path, TParameters parameters, NavigationRequest baseRequest = null);
   }
}