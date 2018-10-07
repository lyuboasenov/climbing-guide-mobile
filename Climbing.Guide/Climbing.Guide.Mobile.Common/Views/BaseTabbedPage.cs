using FreshMvvm;
using SlideOverKit;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Common.Views {
   public class BaseTabbedPage : TabbedPage, IMenuContainerPage {
      public static readonly BindableProperty ChildPagesProperty =
         BindableProperty.Create(nameof(ChildPages), typeof(IEnumerable<Page>), typeof(BaseTabbedPage), null,
            propertyChanged: OnChildPagesChanged);

      private static void OnChildPagesChanged(BindableObject bindable, object oldvalue, object newvalue) {
         ((BaseTabbedPage)bindable).OnChildPagesChanged();
      }

      private void OnChildPagesChanged() {
         Children.Clear();
         if (null != ChildPages) {
            foreach (var childPage in ChildPages) {
               Children.Add(childPage);
            }
         }
      }

      SlideMenuView slideMenu;

      public IEnumerable<Page> ChildPages { get { return (IEnumerable<Page>)GetValue(ChildPagesProperty); } set { SetValue(ChildPagesProperty, value); } }

      public BaseTabbedPage() {
         PopupViews = new Dictionary<string, SlidePopupView>();
      }

      protected override void OnBindingContextChanged() {
         base.OnBindingContextChanged();

         var mapper = (ViewModelMapper)FreshPageModelResolver.PageModelMapper;
         var typeName = mapper.GetPageModelTypeName(GetType());
         Type type = Type.GetType(typeName);

         if (null == BindingContext
             || (type != null && BindingContext.GetType() != type)) {
            var context = Activator.CreateInstance(type);
            (context as ViewModels.BaseViewModel).CurrentPage = this;
            BindingContext = context;
         }

         if (null != SlideMenu) {
            SlideMenu.BindingContext = this.BindingContext;
         }

         if (null != PopupViews) {
            foreach (var popupView in PopupViews.Values) {
               popupView.BindingContext = this.BindingContext;
            }
         }

         SetBinding(ChildPagesProperty, new Binding("Pages"));
      }

      public SlideMenuView SlideMenu {
         get {
            return slideMenu;
         }
         set {
            if (slideMenu != null)
               slideMenu.Parent = null;
            slideMenu = value;
            if (slideMenu != null)
               slideMenu.Parent = this;
         }
      }
      public Dictionary<string, SlidePopupView> PopupViews { get; set; }
      public Action ShowMenuAction { get; set; }
      public Action HideMenuAction { get; set; }
      public Action<string> ShowPopupAction { get; set; }
      public Action HidePopupAction { get; set; }

      public void ShowMenu() {
         if (ShowMenuAction != null)
            ShowMenuAction();
      }

      public void HideMenu() {
         if (HideMenuAction != null)
            HideMenuAction();
      }

      public void ShowPopup(string name) {
         if (ShowPopupAction != null)
            ShowPopupAction(name);
      }

      public void HidePopup() {
         if (HidePopupAction != null)
            HidePopupAction();
      }
   }
}