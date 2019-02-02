using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services.Navigation {
   public interface INavigation {
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
      Task NavigateAsync(INavigationRequest request);

      INavigationRequest GetNavigationRequest(string path, INavigationRequest baseRequest = null);

      INavigationRequest GetNavigationRequest<TParameters>(string path, TParameters parameters, INavigationRequest baseRequest = null);
   }
}