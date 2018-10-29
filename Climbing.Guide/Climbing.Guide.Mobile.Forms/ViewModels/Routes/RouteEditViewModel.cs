using Climbing.Guide.Core.API.Schemas;
using Climbing.Guide.Core.Models.Routes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Climbing.Guide.Mobile.Forms.ViewModels.Routes {
   [PropertyChanged.AddINotifyPropertyChangedInterface]
   public class RouteEditViewModel : Guide.BaseGuideViewModel {
      public static string VmTitle { get; } = Resources.Strings.Routes.Route_Title;

      public ICommand ViewSchemaCommand { get; set; }
      public ICommand SaveCommand { get; set; }
      public ICommand CancelCommand { get; set; }
      public string LocalSchemaThumbPath { get; set; }

      public string Name { get; set; }
      public string Info { get; set; }
      public IGrade SelectedDifficulty { get; set; }
      public IEnumerable<IGrade> Difficulty { get; set; } = GetService<IGradeService>().GetGradeList(GradeType.V);
      public string RouteType { get; set; } = Resources.Strings.Routes.Route_Edit_Route_Type_Boulder;
      public IEnumerable<string> RouteTypes { get; set; }
      public double Length { get; set; }
      public string FA { get; set; }

      public ObservableCollection<Point> SchemaRoute { get; set; } = new ObservableCollection<Point>();

      public MapSpan VisibleRegion { get; set; }

      public RouteEditViewModel() {
         Title = VmTitle;

         ViewSchemaCommand = new Command(async () => await ViewSchema());
      }

      protected override void InitializeCommands() {
         base.InitializeCommands();

         SaveCommand = new Command(Save, CanSave);
         CancelCommand = new Command(async () => await GoBack());
      }

      private async Task GoBack() {
         await NavigationService.GoBackAsync();
      }

      private bool CanSave() {
         return true;
      }

      private void Save() {
         
      }

      public override void OnNavigatedTo(params object[] parameters) {
         base.OnNavigatedTo(parameters);
         SelectedRegion = parameters[0] as Core.API.Schemas.Region;
         SelectedArea = parameters[1] as Area;
         SelectedSector = parameters[2] as Sector;
         LocalSchemaThumbPath = parameters[3] as string;
      }

      public override void OnSelectedSectorChanged() {
         Routes = null;
         SelectedRoute = null;
      }

      private async Task ViewSchema() {
         //await CurrentPage.DisplayAlert("View schema", "SCHEMA!!!!", Resources.Strings.Main.Ok);
      }
   }
}