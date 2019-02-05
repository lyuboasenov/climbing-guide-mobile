using Climbing.Guide.Forms.Services.IoC;

namespace Climbing.Guide.Forms.Commands {
   internal class CommandFactory : ICommandFactory {
      private IContainer Container { get; }

      public CommandFactory(IContainer container) {
         Container = container;
      }

      public TCommand GetCommand<TCommand>() where TCommand : IAsyncCommand {
         return Container.Resolve<TCommand>();
      }
   }
}
