using System;
using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Common.ViewModels {
   public interface ICGMasterDetailNavigationOnlyItem {
      Action<object> NavigationAction { get; }
      Page PageToNavigateTo { get; }
      string PageTitleToNavigateTo { get; }
   }
}
