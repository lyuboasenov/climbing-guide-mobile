using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Tasks {
   public interface IAsyncTaskRunner {
      Task RunAsync(Action action);
      Task<TResult> RunAsync<TResult>(Func<TResult> action);
      Task<TResult> RunAsync<TResult>(Func<Task<TResult>> function);
   }
}