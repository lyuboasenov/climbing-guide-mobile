using Climbing.Guide.Mobile.Common.ViewModels;
using FreshMvvm;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Common.Views {
   public class CGMasterDetailNavigationContainer : FreshMasterDetailNavigationContainer {


      private Dictionary<string, bool> ModalPages { get; set; } = new Dictionary<string, bool>();
      private Dictionary<string, BaseViewModel> ViewModels { get; set; } = new Dictionary<string, BaseViewModel>();
      private string MenuPageTitle { get; set; }
      private string MenuIcon { get; set; }
      private ContentPage MenuPage { get; set; }

      public void AddPage<T>(string title, bool modal, object data = null) where T : FreshBasePageModel {
         this.AddPage<T>(title, data);
         if (!ModalPages.ContainsKey(title)) {
            ModalPages.Add(title, modal);
         }
      }

      public override void AddPage<T>(string title, object data = null) {
         ModalPages.Add(title, false);

         if (typeof(T).GetInterfaces().Contains(typeof(ICGMasterDetailNavigationOnlyItem))) {
            var viewModel = FreshIOC.Container.Resolve(typeof(T)) as BaseViewModel;
            ViewModels.Add(title, viewModel);
            PageNames.Add(title);
         } else {
            base.AddPage<T>(title, data);
            ViewModels.Add(title, Pages[title].GetModel() as BaseViewModel);
         }
      }

      public override void AddPage(string modelName, string title, object data = null) {
         throw new NotSupportedException();
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
                  listView.SelectedItem = null;
               } else {
                  Detail = Pages[selectedTitle];
               }
            } else {
               ICGMasterDetailNavigationOnlyItem viewModel = ViewModels[selectedTitle] as ICGMasterDetailNavigationOnlyItem;
               viewModel.NavigationAction(null);
               Detail = Pages[viewModel.PageTitleToNavigateTo];
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
