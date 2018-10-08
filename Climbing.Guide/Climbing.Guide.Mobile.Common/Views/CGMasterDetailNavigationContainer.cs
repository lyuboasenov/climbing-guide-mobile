using FreshMvvm;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Common.Views {
   public class CGMasterDetailNavigationContainer : FreshMasterDetailNavigationContainer {


      private Dictionary<string, bool> ModalPages { get; set; } = new Dictionary<string, bool>();
      private string MenuPageTitle { get; set; }
      private string MenuIcon { get; set; }
      private ContentPage MenuPage { get; set; }

      public void AddPage<T>(string title, bool modal, object data = null) where T : FreshBasePageModel {
         base.AddPage<T>(title, data);
         ModalPages.Add(title, modal);
      }
      public override void AddPage<T>(string title, object data = null) {
         AddPage<T>(title, false, data);
      }

      public void RemovePage(string title) {
         Pages.Remove(title);
         PageNames.Remove(title);
         ModalPages.Remove(title);

         Init(this.MenuPageTitle, MenuIcon);
      }

      protected override void CreateMenuPage(string menuPageTitle, string menuIcon = null) {
         // Caches menu title and icon in case re-init is invoked
         MenuPageTitle = menuPageTitle;
         MenuIcon = menuIcon;

         MenuPage = new ContentPage();
         MenuPage.Title = menuPageTitle;
         var listView = new ListView();

         listView.ItemsSource = PageNames;

         listView.ItemSelected += (sender, args) => {
            var selectedTitle = (string)args.SelectedItem;
            if (Pages.ContainsKey(selectedTitle)) {
               if (ModalPages[selectedTitle]) {
                  Navigation.PushModalAsync(Pages[selectedTitle]);
               } else {
                  Detail = Pages[selectedTitle];
               }
            }

            IsPresented = false;
         };

         MenuPage.Content = listView;

         var navPage = new NavigationPage(MenuPage) { Title = "Menu" };

         if (!string.IsNullOrEmpty(menuIcon))
            navPage.Icon = menuIcon;

         Master = navPage;
      }
   }
}
