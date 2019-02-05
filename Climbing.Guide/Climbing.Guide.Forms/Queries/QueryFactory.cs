using Climbing.Guide.Forms.Services.IoC;

namespace Climbing.Guide.Forms.Queries {
   internal class QueryFactory : IQueryFactory {
      private IContainer Container { get; }

      public QueryFactory(IContainer container) {
         Container = container;
      }

      public TQuery GetQuery<TQuery>() where TQuery : IAsyncQuery {
         return Container.Resolve<TQuery>();
      }
   }
}
