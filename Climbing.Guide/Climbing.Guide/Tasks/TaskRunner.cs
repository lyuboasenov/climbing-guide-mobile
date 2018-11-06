using Climbing.Guide.Logging;
using Climbing.Guide.Services;
using System;
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
   }
}
