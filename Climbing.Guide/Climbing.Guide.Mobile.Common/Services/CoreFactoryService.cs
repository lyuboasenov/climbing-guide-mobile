using System;
using System.Collections.Generic;
using Xamarin.Forms;

// Register CoreFactoryService in the DependencyService
[assembly: Dependency(typeof(Climbing.Guide.Mobile.Common.Services.CoreFactoryService))]
namespace Climbing.Guide.Mobile.Common.Services {
   internal class CoreFactoryService : ICoreFactoryService {
      private static Dictionary<Type, Type> Registry { get; set; } = new Dictionary<Type, Type>();

      public void Register<T, U>() where U : new() {
         Registry.Add(typeof(T), typeof(U));
      }

      static CoreFactoryService() {
         Registry.Add(typeof(Core.API.IRestApiClientSettings), typeof(Core.API.RestApiClientSettings));
      }

      public T GetObject<T>() {
         Type requestedType = typeof(T);

         if (!Registry.ContainsKey(requestedType)) {
            // If same type as requested not found. An attempt is done to find 
            // a child type of the requested.
            requestedType = null;
            foreach (var type in Registry.Keys) {
               if (typeof(T).IsAssignableFrom(type)) {
                  requestedType = type;
                  break;
               }
            }

            if (null == requestedType) {
               throw new ArgumentException($"Requested type {typeof(T)} not registered.");
            }
         }

         return (T)Activator.CreateInstance(Registry[requestedType]);
      }
   }
}
