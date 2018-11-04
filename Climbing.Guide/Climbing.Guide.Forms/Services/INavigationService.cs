using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services {
   public interface INavigationService {
      Uri GetNavigationUri(string absolutePath);
      Uri GetShellNavigationUri(string relativePath);

      //
      // Summary:
      //     Navigates to the most recent entry in the back navigation history by popping
      //     the calling Page off the navigation stack.
      //
      // Returns:
      //     If true a go back operation was successful. If false the go back operation failed.
      Task<ITaskResult<bool>> GoBackAsync();
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
      Task<ITaskResult<bool>> GoBackAsync(params object[] parameters);

      Task<ITaskResult<bool>> NavigateAsync(Uri uri);
      Task<ITaskResult<bool>> NavigateAsync(Uri uri, params object[] parameters);
   }
}