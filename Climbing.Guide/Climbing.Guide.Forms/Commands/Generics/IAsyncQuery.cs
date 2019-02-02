using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Commands.Generics {
   public interface IAsyncQuery<TResult> : IAsyncQuery {
      new Task<TResult> GetResultAsync();
   }
}
