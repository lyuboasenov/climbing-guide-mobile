using Climbing.Guide.Forms.Services.IoC;

namespace Climbing.Guide.Forms.Queries {
   internal class QueryFactory : IQueryFactory {
      private IContainerProvider Container { get; }

      public QueryFactory(IContainerProvider container) {
         Container = container;
      }

      public TQuery GetQuery<TQuery>() where TQuery : IAsyncQuery {
         return Container.Resolve<TQuery>();
      }
   }
}
