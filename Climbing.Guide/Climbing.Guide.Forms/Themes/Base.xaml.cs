using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Climbing.Guide.Forms.Themes {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class Base : ResourceDictionary {
      public Base() {
         InitializeComponent();
      }
   }
}