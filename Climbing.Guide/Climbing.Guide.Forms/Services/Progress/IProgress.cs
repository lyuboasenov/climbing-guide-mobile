using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services.Progress {
   public interface IProgress {
      Task<ILoadingSession> CreateLoadingSessionAsync();
      Task<IProgressSession> CreateProgressSessionAsync();
   }
}
