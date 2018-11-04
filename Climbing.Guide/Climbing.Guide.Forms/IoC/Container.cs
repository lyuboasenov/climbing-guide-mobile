using Prism.Ioc;
using System;

namespace Climbing.Guide.Forms.IoC {
   public class Container : IContainerExtension {

      private IContainerExtension InternalContainerExtension { get; set; }

      public bool SupportsModules => InternalContainerExtension.SupportsModules;

      public Container(IContainerExtension containerExtension) {
         InternalContainerExtension = containerExtension;
      }

      public static T Get<T>() where T : class {
         return Prism.PrismApplicationBase.Current.Container.Resolve<T>();
      }

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
