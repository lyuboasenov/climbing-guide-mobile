using Prism.Ioc;
using System;

namespace Climbing.Guide.Forms.Services.IoC {
   public class Container : IContainerExtension, IContainerRegistry, IContainerProvider {

      private IContainerExtension InternalContainerExtension { get; }

      public Container(IContainerExtension containerExtension) {
         InternalContainerExtension = containerExtension;
      }

      public static T Get<T>() where T : class {
         var container = Prism.PrismApplicationBase.Current.Container;
         return container.Resolve<T>();
      }

      public void FinalizeExtension() {
         InternalContainerExtension.FinalizeExtension();
      }

      // Registry

      public IContainerRegistry Register<T, U>() {
         InternalContainerExtension.Register(typeof(T), typeof(U));
         return this;
      }

      public IContainerRegistry Register<T, U>(string name) {
         InternalContainerExtension.Register(typeof(T), typeof(U), name);
         return this;
      }

      public IContainerRegistry Register(Type from, Type to) {
         InternalContainerExtension.Register(from, to);
         return this;
      }

      public IContainerRegistry Register(Type from, Type to, string name) {
         InternalContainerExtension.Register(from, to, name);
         return this;
      }

      public IContainerRegistry RegisterInstance<T>(T instance) {
         InternalContainerExtension.RegisterInstance(typeof(T), instance);
         return this;
      }

      public IContainerRegistry RegisterInstance(Type type, object instance) {
         InternalContainerExtension.RegisterInstance(type, instance);
         return this;
      }

      public IContainerRegistry RegisterSingleton<T, U>() {
         InternalContainerExtension.RegisterSingleton(typeof(T), typeof(U));
         return this;
      }

      public IContainerRegistry RegisterSingleton(Type from, Type to) {
         InternalContainerExtension.RegisterSingleton(from, to);
         return this;
      }

      // Provider

      public T Resolve<T>() {
         return (T)InternalContainerExtension.Resolve(typeof(T));
      }

      public T Resolve<T>(string name) {
         return (T)InternalContainerExtension.Resolve(typeof(T), name);
      }

      public object Resolve(Type type) {
         return InternalContainerExtension.Resolve(type);
      }

      public object Resolve(Type type, string name) {
         return InternalContainerExtension.Resolve(type, name);
      }

      // Prism container

      public object Resolve(Type type, params (Type Type, object Instance)[] parameters) {
         return InternalContainerExtension.Resolve(type, parameters);
      }

      public object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters) {
         return InternalContainerExtension.Resolve(type, name, parameters);

      }

      Prism.Ioc.IContainerRegistry Prism.Ioc.IContainerRegistry.RegisterInstance(Type type, object instance) {
         return InternalContainerExtension.RegisterInstance(type, instance);
      }

      public Prism.Ioc.IContainerRegistry RegisterInstance(Type type, object instance, string name) {
         return InternalContainerExtension.RegisterInstance(type, instance, name);
      }

      Prism.Ioc.IContainerRegistry Prism.Ioc.IContainerRegistry.RegisterSingleton(Type from, Type to) {
         return InternalContainerExtension.RegisterSingleton(from, to);
      }

      public Prism.Ioc.IContainerRegistry RegisterSingleton(Type from, Type to, string name) {
         return InternalContainerExtension.RegisterSingleton(from, to, name);
      }

      Prism.Ioc.IContainerRegistry Prism.Ioc.IContainerRegistry.Register(Type from, Type to) {
         return InternalContainerExtension.Register(from, to);
      }

      Prism.Ioc.IContainerRegistry Prism.Ioc.IContainerRegistry.Register(Type from, Type to, string name) {
         return InternalContainerExtension.Register(from, to, name);
      }

      public bool IsRegistered(Type type) {
         return InternalContainerExtension.IsRegistered(type);
      }

      public bool IsRegistered(Type type, string name) {
         return InternalContainerExtension.IsRegistered(type, name);
      }
   }
}
