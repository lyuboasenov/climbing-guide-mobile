using Climbing.Guide.Api.Schemas;
using System;
using Xamarin.Forms;

namespace Climbing.Guide.Forms.Forms {
   public class GuideDataTemplateSelector : DataTemplateSelector {

      public DataTemplate AreaTemplate { get; set; }
      public DataTemplate RouteTemplate { get; set; }

      protected override DataTemplate OnSelectTemplate(object item, BindableObject container) {
         DataTemplate template = null;
         if (item is Area) {
            template = AreaTemplate;
         } else if (item is Route) {
            template = RouteTemplate;
         } else {
            throw new ArgumentException($"Unsupported item type {item.GetType()}");
         }

         return template;
      }
   }
}
