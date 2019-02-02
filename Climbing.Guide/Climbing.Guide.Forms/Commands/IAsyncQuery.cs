using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Commands {
   public interface IAsyncQuery {
      Task<object> GetResultAsync();
   }
}
