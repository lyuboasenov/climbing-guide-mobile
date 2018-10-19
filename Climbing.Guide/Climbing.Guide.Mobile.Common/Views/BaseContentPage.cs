using System;
using FreshMvvm;

using SlideOverKit;

namespace Climbing.Guide.Mobile.Common.Views {
   public class BaseContentPage : MenuContainerPage {

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
      }
   }
}
