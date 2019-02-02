namespace Climbing.Guide.Forms.Commands.Generics {
   public interface IQuery<out TResult> : IQuery {
      new TResult GetResult();
   }
}
