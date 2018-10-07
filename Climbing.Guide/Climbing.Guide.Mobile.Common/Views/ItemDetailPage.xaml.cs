using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Climbing.Guide.Mobile.Common.Models;
using Climbing.Guide.Mobile.Common.ViewModels;

namespace Climbing.Guide.Mobile.Common.Views {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class ItemDetailPage : BaseContentPage {

      public ItemDetailPage() {
         InitializeComponent();
      }
   }
}