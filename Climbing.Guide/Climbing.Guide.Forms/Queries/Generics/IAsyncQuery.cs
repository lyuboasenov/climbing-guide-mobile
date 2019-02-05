using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Queries.Generics {
   public interface IAsyncQuery<TResult> : IAsyncQuery {
      new Task<TResult> GetResultAsync();
   }
}
