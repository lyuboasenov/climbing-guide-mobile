using System;

namespace Climbing.Guide.Forms.Services.IoC {
   public interface IContainerRegistry {
      IContainerRegistry Register<T, U>();
      IContainerRegistry Register<T, U>(string name);

      IContainerRegistry Register(Type from, Type to);
      IContainerRegistry Register(Type from, Type to, string name);

      IContainerRegistry RegisterInstance(Type type, object instance);
      IContainerRegistry RegisterInstance<T>(T instance);

      IContainerRegistry RegisterSingleton<T, U>();
      IContainerRegistry RegisterSingleton(Type from, Type to);

   }
}