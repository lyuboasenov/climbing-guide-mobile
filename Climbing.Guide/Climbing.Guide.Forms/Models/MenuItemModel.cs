using Climbing.Guide.Forms.Services.Navigation;

namespace Climbing.Guide.Forms.Models {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class MenuItemModel {
      public string Title { get; set; }
      public string Group { get; set; }
      public INavigationRequest NavigationRequest { get; set; }
   }
}
