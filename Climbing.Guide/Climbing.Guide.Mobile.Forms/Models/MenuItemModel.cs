using System;

namespace Climbing.Guide.Mobile.Forms.Models {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class MenuItemModel {
      public string Title { get; set; }
      public string Group { get; set; }
      public Uri NavigationUri { get; set; }
   }
}
