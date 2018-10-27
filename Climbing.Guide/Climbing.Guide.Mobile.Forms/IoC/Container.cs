using Prism.Ioc;
using System;

namespace Climbing.Guide.Mobile.Forms.IoC {
   public class Container : IContainerExtension {

      //private static Dictionary<Type, Type> TypeRegistry { get; set; } = new Dictionary<Type, Type>();
      //private static Dictionary<Type, object> ObjectRegistry { get; set; } = new Dictionary<Type, object>();

      private IContainerExtension InternalContainerExtension { get; set; }

      public bool SupportsModules => InternalContainerExtension.SupportsModules;

      public Container(IContainerExtension containerExtension) {
         InternalContainerExtension = containerExtension;
      }

      //public static void Register<T, U>() where U : new() {
      //   Register(typeof(T), typeof(U));
      //}

      //public static void Register<T>(T service) {
      //   Register(typeof(T), service.GetType());
      //   ObjectRegistry[typeof(T)] = service;
      //}

      public static T Get<T>() where T : class {
         //T result = null;
         //Type requestedType = typeof(T);

         //// Tries to get already created object
         //if (ObjectRegistry.ContainsKey(requestedType)) {
         //   result = (T)ObjectRegistry[requestedType];
         //}

         //// Tries get type from type registry and create new object. If type is found the newly create object is cached
         //if (null == result && TypeRegistry.ContainsKey(requestedType)) {
         //   result = (T)Activator.CreateInstance(TypeRegistry[requestedType]);
         //   Register(result);
         //}

         //// Tries to get the object from dependency service
         //if (null == result) {
         //   result = DependencyService.Get<T>();
         //   if (null != result) {
         //      // Service found in the dependency service object will be cached
         //      Register(result);
         //   }
         //}

         //if (null == result) {
         //   throw new ArgumentException($"Requested type {typeof(T)} not registered.");
         //}

         //return result;

         return Prism.PrismApplicationBase.Current.Container.Resolve<T>();
      }

      //private static void Register(Type key, Type value) {
      //   TypeRegistry[key] = value;
      //}

      public void FinalizeExtension() {
         InternalContainerExtension.FinalizeExtension();
      }

      public object ResolveViewModelForView(object view, Type viewModelType) {
         return InternalContainerExtension.ResolveViewModelForView(view, viewModelType);
      }

      public object Resolve(Type type) {
         return InternalContainerExtension.Resolve(type);
      }

      public object Resolve(Type type, string name) {
         return InternalContainerExtension.Resolve(type, name);
      }

      public void RegisterInstance(Type type, object instance) {
         InternalContainerExtension.RegisterInstance(type, instance);
      }

      public void RegisterSingleton(Type from, Type to) {
         InternalContainerExtension.RegisterSingleton(from, to);
      }

      void IContainerRegistry.Register(Type from, Type to) {
         InternalContainerExtension.Register(from, to);
      }

      public void Register(Type from, Type to, string name) {
         InternalContainerExtension.Register(from, to, name);
      }

      // ----------------------------------------------------------------

      public T Resolve<T>() {
         return (T)InternalContainerExtension.Resolve(typeof(T));
      }

      public T Resolve<T>(string name) {
         return (T)InternalContainerExtension.Resolve(typeof(T), name);
      }

      public void RegisterInstance<T>(T instance) {
         InternalContainerExtension.RegisterInstance(typeof(T), instance);
      }

      public void RegisterSingleton<T, U>() {
         InternalContainerExtension.RegisterSingleton(typeof(T), typeof(U));
      }

      public void Register<T, U>() {
         InternalContainerExtension.Register(typeof(T), typeof(U));
      }

      public void Register<T, U>(string name) {
         InternalContainerExtension.Register(typeof(T), typeof(U), name);
      }
   }
}
