namespace Climbing.Guide.Forms.Queries.Generics {
   public interface IQuery<out TResult> : IQuery {
      new TResult GetResult();
   }
}
