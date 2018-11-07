﻿using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Tasks {
   public interface ITaskRunner {
      Task Run(Action action);
      Task<TResult> Run<TResult>(Func<TResult> action);
      Task<TResult> Run<TResult>(Func<Task<TResult>> function);

      T RunSync<T>(Func<Task<T>> task);
      void RunSync(Func<Task> task);
   }
}