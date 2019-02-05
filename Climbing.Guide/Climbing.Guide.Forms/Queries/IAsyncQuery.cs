using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Queries {
   public interface IAsyncQuery {
      Task<object> GetResultAsync();
   }
}
