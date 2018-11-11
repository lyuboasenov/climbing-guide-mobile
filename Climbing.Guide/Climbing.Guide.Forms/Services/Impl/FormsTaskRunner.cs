using Climbing.Guide.Logging;
using Climbing.Guide.Services;
using Climbing.Guide.Tasks;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Climbing.Guide.Forms.Services {
   public class FormsTaskRunner : TaskRunner {

      public FormsTaskRunner(ILogger logger, IErrorService errorService) :base (logger, errorService) {

      }

      public override Task RunOnUIThreadAsync(Action action) {
         Func<Task> asyncAction = async () => { action(); };

         return RunOnUIThreadAsync(asyncAction);
      }

      public override Task<TResult> RunOnUIThreadAsync<TResult>(Func<TResult> function) {
         Func<Task<TResult>> asyncFunc = async () => { return function(); };

         return RunOnUIThreadAsync(asyncFunc);
      }

      public override Task<TResult> RunOnUIThreadAsync<TResult>(Func<Task<TResult>> function) {
         Task<TResult> result = null;
         Device.BeginInvokeOnMainThread(() => { result = function(); });

         return result;
      }
   }
}
