using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services.Progress {
   public interface Progress {
      Task<LoadingSession> CreateLoadingSessionAsync();
      Task<ProgressSession> CreateProgressSessionAsync();
   }
}
