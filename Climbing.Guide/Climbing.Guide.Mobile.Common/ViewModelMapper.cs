using System;
using FreshMvvm;

namespace Climbing.Guide.Mobile.Common {
   class ViewModelMapper : IFreshPageModelMapper {
      public string GetPageTypeName(Type pageModelType) {
         return pageModelType.AssemblyQualifiedName
            .Replace(".ViewModels.", ".Views.")
            .Replace("ViewModel", "View")
            .Replace("PageModel", "Page");
      }

      public string GetPageModelTypeName(Type pageType) {
         return pageType.AssemblyQualifiedName
            .Replace(".Common.", ".Common.ViewModels.")
            .Replace("View,", "ViewModel,")
            .Replace("Page,", "PageModel,");
      }
   }
}
