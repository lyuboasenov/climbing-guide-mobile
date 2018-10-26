using System;
using System.Collections.Generic;
using System.Text;

namespace Climbing.Guide.Mobile.Common.Models {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class MenuItemModel {
      public string Title { get; set; }
      public string Group { get; set; }
      public Uri NavigationUri { get; set; }
   }
}
