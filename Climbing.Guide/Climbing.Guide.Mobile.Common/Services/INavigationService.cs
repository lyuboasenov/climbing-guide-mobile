using System.Threading.Tasks;
using FreshMvvm;

namespace Climbing.Guide.Mobile.Common.Services {
   internal interface INavigationService {
      void InitializeNavigation();
      //Task PopAsync();
      //Task PushAsync<T>(object data = null) where T : FreshBasePageModel;
      void UpdateNavigationContainer();
      //void SendBackButtonPressed();
   }
}