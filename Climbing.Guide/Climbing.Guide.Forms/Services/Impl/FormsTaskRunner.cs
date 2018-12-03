using Climbing.Guide.Logging;
using Climbing.Guide.Tasks;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Climbing.Guide.Forms.Services {
   public class FormsTaskRunner : TaskRunner, IMainThreadTaskRunner {

      public FormsTaskRunner(ILogger logger, Exceptions.IExceptionHandler exceptionHandler) : 
         base (logger, exceptionHandler) { }

      public Task RunOnUIThreadAsync(Action action) {
         Func<Task> asyncAction = async () => { action(); };

         return RunOnUIThreadAsync(asyncAction);
      }

      public Task<TResult> RunOnUIThreadAsync<TResult>(Func<TResult> function) {
         Func<Task<TResult>> asyncFunc = async () => { return function(); };

         return RunOnUIThreadAsync(asyncFunc);
      }

      public Task<TResult> RunOnUIThreadAsync<TResult>(Func<Task<TResult>> function) {
         Task<TResult> result = null;
         Device.BeginInvokeOnMainThread(() => { result = function(); });

         return result;
      }
   }
}
