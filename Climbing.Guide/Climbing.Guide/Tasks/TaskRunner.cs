﻿using Alat.Logging;
using Alat.Logging.Appenders;
using Alat.Logging.DataConverters;
using Alat.Logging.Factories;
using Alat.Logging.LogEntryFormatters;
using Climbing.Guide.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Climbing.Guide.Tasks {
   public class TaskRunner : ISyncTaskRunner, IAsyncTaskRunner {
      protected ILogger Logger { get; set; }
      protected IExceptionHandler ExceptionHandler { get; set; }

      public TaskRunner() : this(LoggerFactory.GetDebugLogger(Level.All), new VoidExceptionHandler()) { }

      public TaskRunner(ILogger logger, IExceptionHandler exceptionHandler) {
         Logger = logger;
         ExceptionHandler = exceptionHandler;
      }

      public async Task RunAsync(Action action) {
         try {
            await Task.Run(action);
         } catch(Exception ex) {
            await ExceptionHandler.HandleAsync(ex, "Error executing task.");
            Logger.Log(ex);
            throw;
         }
      }

      public async Task<TResult> RunAsync<TResult>(Func<TResult> action) {
         try {
            return await Task.Run(action);
         } catch (Exception ex) {
            await ExceptionHandler.HandleAsync(ex, "Error executing task.");
            Logger.Log(ex);
            throw;
         }
      }

      public async Task<TResult> RunAsync<TResult>(Func<Task<TResult>> function) {
         try {
            return await Task.Run(function);
         } catch (Exception ex) {
            await ExceptionHandler.HandleAsync(ex, "Error executing task.");
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
