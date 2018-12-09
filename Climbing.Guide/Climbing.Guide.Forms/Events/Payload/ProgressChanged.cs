using System;
using System.Collections.Generic;
using System.Text;

namespace Climbing.Guide.Forms.Events.Payload {
   public class ProgressChanged {
      public string Message { get; set; }
      public double Processed { get; set; }
      public double Total { get; set; }
   }
}
