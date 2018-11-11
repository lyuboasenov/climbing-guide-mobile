using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Tasks {
   public interface ITaskRunner {
      Task RunOnUIThreadAsync(Action action);
      Task<TResult> RunOnUIThreadAsync<TResult>(Func<TResult> function);
      Task<TResult> RunOnUIThreadAsync<TResult>(Func<Task<TResult>> function);

      Task RunAsync(Action action);
      Task<TResult> RunAsync<TResult>(Func<TResult> action);
      Task<TResult> RunAsync<TResult>(Func<Task<TResult>> function);

      T RunSync<T>(Func<Task<T>> task);
      void RunSync(Func<Task> task);
   }
}