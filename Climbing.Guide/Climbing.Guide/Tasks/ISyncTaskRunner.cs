using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Tasks {
   public interface ISyncTaskRunner {
      T RunSync<T>(Func<Task<T>> task);
      void RunSync(Func<Task> task);
   }
}