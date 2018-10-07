using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Climbing.Guide.Mobile.Common.Models;

namespace Climbing.Guide.Mobile.Common.Views {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class NewItemPage : BaseContentPage {
      public Item Item { get; set; }

      public NewItemPage() {
         InitializeComponent();
      }

      //async void Save_Clicked(object sender, EventArgs e) {
      //   MessagingCenter.Send(this, "AddItem", Item);
      //   await Navigation.PopModalAsync();
      //}

      //async void Cancel_Clicked(object sender, EventArgs e) {
      //   await Navigation.PopModalAsync();
      //}
   }
}