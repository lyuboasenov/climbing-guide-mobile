using System;

namespace Climbing.Guide.Mobile.Forms.Services {
   public interface ITaskResult<TResult> {
      TResult Result { get; }
      Exception Exception { get; }
   }
}
