using Climbing.Guide.Logging;
using Climbing.Guide.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Climbing.Guide.Tasks {
   public class TaskRunner : ITaskRunner {
      private ILogger Logger { get; set; }
      private IErrorService ErrorService { get; set; }

      public TaskRunner(ILogger logger, IErrorService errorService) {
         Logger = logger;
         ErrorService = errorService;
      }

      public async Task Run(Action action) {
         try {
            await Task.Run(action);
         } catch(Exception ex) {
            await ErrorService.HandleExceptionAsync(ex, "Error executing task.");
            Logger.Log(ex);
            throw;
         }
      }

      public async Task<TResult> Run<TResult>(Func<TResult> action) {
         try {
            return await Task.Run(action);
         } catch (Exception ex) {
            await ErrorService.HandleExceptionAsync(ex, "Error executing task.");
            Logger.Log(ex);
            throw;
         }
      }

      public async Task<TResult> Run<TResult>(Func<Task<TResult>> function) {
         try {
            return await Task.Run(function);
         } catch (Exception ex) {
            await ErrorService.HandleExceptionAsync(ex, "Error executing task.");
            Logger.Log(ex);
            throw;
         }
      }

      /// <summary>
      /// Execute's an async Task<T> method which has a void return value synchronously
      /// From: https://stackoverflow.com/questions/5095183/how-would-i-run-an-async-taskt-method-synchronously
      /// </summary>
      /// <param name="task">Task<T> method to execute</param>
      public void RunSync(Func<Task> task) {
         var oldContext = SynchronizationContext.Current;
         var synch = new ExclusiveSynchronizationContext();
         SynchronizationContext.SetSynchronizationContext(synch);
         synch.Post(async _ =>
         {
            try {
               await task();
            } catch (Exception e) {
               synch.InnerException = e;
               throw;
            } finally {
               synch.EndMessageLoop();
            }
         }, null);
         synch.BeginMessageLoop();

         SynchronizationContext.SetSynchronizationContext(oldContext);
      }

      /// <summary>
      /// Execute's an async Task<T> method which has a T return type synchronously
      /// From: https://stackoverflow.com/questions/5095183/how-would-i-run-an-async-taskt-method-synchronously
      /// </summary>
      /// <typeparam name="T">Return Type</typeparam>
      /// <param name="task">Task<T> method to execute</param>
      /// <returns></returns>
      public T RunSync<T>(Func<Task<T>> task) {
         var oldContext = SynchronizationContext.Current;
         var synch = new ExclusiveSynchronizationContext();
         SynchronizationContext.SetSynchronizationContext(synch);
         T ret = default(T);
         synch.Post(async _ =>
         {
            try {
               ret = await task();
            } catch (Exception e) {
               synch.InnerException = e;
               throw;
            } finally {
               synch.EndMessageLoop();
            }
         }, null);
         synch.BeginMessageLoop();
         SynchronizationContext.SetSynchronizationContext(oldContext);
         return ret;
      }
   }
}
