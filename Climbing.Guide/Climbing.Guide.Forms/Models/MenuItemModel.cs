using System;

namespace Climbing.Guide.Forms.Models {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class MenuItemModel {
      public string Title { get; set; }
      public string Group { get; set; }
      public Uri NavigationUri { get; set; }
   }
}
