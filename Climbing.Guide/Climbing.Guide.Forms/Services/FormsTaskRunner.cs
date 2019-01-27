using Alat.Logging;
using Climbing.Guide.Tasks;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Climbing.Guide.Forms.Services {
   public class FormsTaskRunner : TaskRunner, IMainThreadTaskRunner {

      public FormsTaskRunner(ILogger logger) : base (logger) { }

      public Task RunOnUIThreadAsync(Action action) {
         Func<Task> asyncAction = () => {
            try {
               action();
               return Task.CompletedTask;
            } catch(Exception ex) {
               return Task.FromException(ex);
            }
         };

         return RunOnUIThreadAsync(asyncAction);
      }

      public Task<TResult> RunOnUIThreadAsync<TResult>(Func<TResult> function) {
         Func<Task<TResult>> asyncFunc = () => {
            try {
               return Task.FromResult(function());
            } catch (Exception ex) {
               return Task.FromException<TResult>(ex);
            }
         };

         return RunOnUIThreadAsync(asyncFunc);
      }

      public Task<TResult> RunOnUIThreadAsync<TResult>(Func<Task<TResult>> function) {
         TaskCompletionSource<TResult> resultTask = new TaskCompletionSource<TResult>();

         Device.BeginInvokeOnMainThread(async () => {
            try {
               var result = await function();
               resultTask.SetResult(result);
            } catch (Exception ex) {
               resultTask.SetException(ex);
            }
         });

         return resultTask.Task;
      }
   }
}
