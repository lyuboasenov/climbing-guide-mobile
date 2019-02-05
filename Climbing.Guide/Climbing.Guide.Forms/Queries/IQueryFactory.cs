namespace Climbing.Guide.Forms.Queries {
   public interface IQueryFactory {
      TQuery GetQuery<TQuery>() where TQuery : IAsyncQuery;
   }
}