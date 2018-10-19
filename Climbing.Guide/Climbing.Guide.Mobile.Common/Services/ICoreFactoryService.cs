namespace Climbing.Guide.Mobile.Common.Services {
   internal interface ICoreFactoryService {
      T GetObject<T>();
      void Register<T, U>() where U : new();
   }
}