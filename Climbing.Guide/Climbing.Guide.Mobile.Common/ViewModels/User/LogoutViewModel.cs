using Climbing.Guide.Core.API.Schemas;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Common.ViewModels.User {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class LogoutViewModel : BaseViewModel, ICGMasterDetailNavigationOnlyItem {
      public static string VmTitle { get; } = Resources.Strings.User.Logout_Title;

      public Action<object> NavigationAction => data => {
         Task.Run(Logout);
      };

      public Page PageToNavigateTo => throw new NotImplementedException();

      public string PageTitleToNavigateTo => Resources.Strings.Main.Home_Title;

      public LogoutViewModel() {
         Title = VmTitle;
      }

      private async Task Logout() {
         bool success = false;
         try {
            success = await Client.LogoutAsync();
         } catch (RestApiCallException ex) {
            await HandleRestApiCallException(ex);
         }

         Navigation.UpdateNavigationContainer();
      }
   }
}