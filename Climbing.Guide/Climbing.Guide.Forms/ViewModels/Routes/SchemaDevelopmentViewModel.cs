using Climbing.Guide.Collections.ObjectModel;
using Climbing.Guide.Forms.Services.Media;
using Climbing.Guide.Forms.Services.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Climbing.Guide.Forms.ViewModels.Routes {

   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class SchemaDevelopmentViewModel : BaseViewModel {
      private readonly IMedia _media;

      public static string VmTitle { get; } = Resources.Strings.Main.About_Title;
      public static INavigationRequest GetNavigationRequest(Services.Navigation.INavigation navigation) {
         return navigation.GetNavigationRequest(nameof(Views.Routes.SchemaDevelopmentView));
      }

      public ObservableCollection<Point> SchemaRoute { get; set; }
      public string LocalSchemaThumbPath { get; set; }
      public ICommand OpenWebCommand { get; }
      public ICommand Command { get; }

      public SchemaDevelopmentViewModel(IMedia media) {
         Title = VmTitle;

         SchemaRoute = new ObservableCollection<Point>();
         OpenWebCommand = new Command(() => Launcher.OpenAsync(new Uri("https://xamarin.com/platform")));
         Command = new Command(async () => await PickImageAsync());
         _media = media ?? throw new ArgumentNullException(nameof(media));

         LocalSchemaThumbPath = "/storage/emulated/0/Android/data/com.climbing.guide.client.droid/files/Pictures/temp/dsc_0760.jpg";
      }



      private async Task PickImageAsync() {
         //var imagePath = await _media.PickPhotoAsync();
         //LocalSchemaThumbPath = imagePath;
      }
   }
}
