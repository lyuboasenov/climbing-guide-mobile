using System;

namespace Climbing.Guide.Forms.Services.IoC {
   public interface IContainerProvider {
      T Resolve<T>();
      T Resolve<T>(string name);

      object Resolve(Type type);
      object Resolve(Type type, string name);
   }
}
