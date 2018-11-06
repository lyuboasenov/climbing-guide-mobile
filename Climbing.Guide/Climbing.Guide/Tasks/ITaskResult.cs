using System;

namespace Climbing.Guide.Tasks {
   public interface ITaskResult<TResult> {
      TResult Result { get; }
      Exception Exception { get; }
   }
}
