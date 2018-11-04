﻿using System;

namespace Climbing.Guide.Forms.Services {
   public interface ITaskResult<TResult> {
      TResult Result { get; }
      Exception Exception { get; }
   }
}