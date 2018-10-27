using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Climbing.Guide.Mobile.Forms.Resources
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class FontFileResources : ResourceDictionary {

      private static readonly FontFileResources _instance = new FontFileResources();
      public FontFileResources() {
         InitializeComponent();
      }

      public static string FontAwesomeRegular => _instance.GetStringResourceForPlatform("FontAwesomeRegularId");
      public static string FontAwesomeSolid => _instance.GetStringResourceForPlatform("FontAwesomeSolidId");
      public static string FontAwesomeBrands => _instance.GetStringResourceForPlatform("FontAwesomeBrandsId");

      private string GetStringResourceForPlatform(string resourceKey) {
         if (!_instance.ContainsKey(resourceKey)) return null;

         if (!(_instance[resourceKey] is OnPlatform<string> resource)) return string.Empty;

         var retString = resource.Platforms.Where(c => c.Platform.Contains(Device.RuntimePlatform))
             .Select(c => c.Value).FirstOrDefault() as string;

         return retString ?? "NOFONT";
      }

   }
}