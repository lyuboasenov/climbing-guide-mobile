using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Tasks {
   public interface IMainThreadTaskRunner {
      Task RunOnUIThreadAsync(Action action);
      Task<TResult> RunOnUIThreadAsync<TResult>(Func<TResult> function);
      Task<TResult> RunOnUIThreadAsync<TResult>(Func<Task<TResult>> function);
   }
}