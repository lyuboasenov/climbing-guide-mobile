using System.Threading;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services.GeoLocation {
   public interface GeoLocation {
      Task<Location> GetCurrentOrDefaultAsync(CancellationToken token = default(CancellationToken));
   }
}
