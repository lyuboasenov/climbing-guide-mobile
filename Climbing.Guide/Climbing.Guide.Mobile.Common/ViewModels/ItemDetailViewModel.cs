using System;

using Climbing.Guide.Mobile.Common.Models;

namespace Climbing.Guide.Mobile.Common.ViewModels {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class ItemDetailViewModel : BaseViewModel {
      public Item Item { get; set; }
      public ItemDetailViewModel(Item item = null) {
         Title = item?.Text;
         Item = item;
      }
   }
}
