using Climbing.Guide.Forms.Services.IoC;

namespace Climbing.Guide.Forms.Commands {
   internal class CommandFactory : ICommandFactory {
      private IContainerProvider Container { get; }

      public CommandFactory(IContainerProvider container) {
         Container = container;
      }

      public TCommand GetCommand<TCommand>() where TCommand : IAsyncCommand {
         return Container.Resolve<TCommand>();
      }
   }
}
