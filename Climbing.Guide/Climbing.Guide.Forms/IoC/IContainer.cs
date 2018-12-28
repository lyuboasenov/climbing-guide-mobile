using System;

namespace Climbing.Guide.Forms.IoC {
   public interface IContainer {
      void Register(Type from, Type to, string name);
      void Register<T, U>();
      void Register<T, U>(string name);
      void RegisterInstance(Type type, object instance);
      void RegisterInstance<T>(T instance);
      void RegisterSingleton(Type from, Type to);
      void RegisterSingleton<T, U>();
      object Resolve(Type type);
      object Resolve(Type type, string name);
      T Resolve<T>();
      T Resolve<T>(string name);
   }
}