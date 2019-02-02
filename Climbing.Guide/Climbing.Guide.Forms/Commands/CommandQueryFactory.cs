using Climbing.Guide.Forms.Services.IoC;

namespace Climbing.Guide.Forms.Commands {
   internal class CommandQueryFactory : ICommandQueryFactory {
      private IContainer Container { get; }

      public CommandQueryFactory(IContainer container) {
         Container = container;
      }

      public TCommand GetCommand<TCommand>() where TCommand : IAsyncCommand {
         return Container.Resolve<TCommand>();
      }

      public TQuery GetQuery<TQuery>() where TQuery : IAsyncQuery {
         return Container.Resolve<TQuery>();
      }
   }
}
