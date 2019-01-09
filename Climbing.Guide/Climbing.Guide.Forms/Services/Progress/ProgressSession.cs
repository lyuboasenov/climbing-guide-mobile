using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services.Progress {
   public interface ProgressSession : IDisposable {
      Task UpdateProgressAsync(double progress, double total, string message);
   }
}
