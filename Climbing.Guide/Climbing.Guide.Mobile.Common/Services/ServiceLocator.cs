using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Climbing.Guide.Mobile.Common.Services {
   internal static class ServiceLocator {

      private static Dictionary<Type, Type> TypeRegistry { get; set; } = new Dictionary<Type, Type>();
      private static Dictionary<Type, object> ObjectRegistry { get; set; } = new Dictionary<Type, object>();

      public static void Register<T, U>() where U : new() {
         Register(typeof(T), typeof(U));
      }

      public static void Register<T>(T service) {
         Register(typeof(T), service.GetType());
         ObjectRegistry[typeof(T)] = service;
      }

      public static T Get<T>() where T : class {
         T result = null;
         Type requestedType = typeof(T);

         // Tries to get already created object
         if (ObjectRegistry.ContainsKey(requestedType)) {
            result = (T)ObjectRegistry[requestedType];
         }

         // Tries get type from type registry and create new object. If type is found the newly create object is cached
         if (null == result && TypeRegistry.ContainsKey(requestedType)) {
            result = (T)Activator.CreateInstance(TypeRegistry[requestedType]);
            Register(result);
         }

         // Tries to get the object from dependency service
         if (null == result) {
            result = DependencyService.Get<T>();
            if (null != result) {
               // Service found in the dependency service object will be cached
               Register(result);
            }
         }

         if (null == result) {
            throw new ArgumentException($"Requested type {typeof(T)} not registered.");
         }

         return result;
      }

      private static void Register(Type key, Type value) {
         TypeRegistry[key] = value;
      }
   }
}
