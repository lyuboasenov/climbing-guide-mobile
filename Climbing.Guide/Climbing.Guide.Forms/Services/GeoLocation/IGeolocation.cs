using System.Threading;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services.GeoLocation {
   public interface IGeoLocation {
      Task<ILocation> GetCurrentOrDefaultAsync(CancellationToken token = default(CancellationToken));
   }
}
