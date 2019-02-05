namespace Climbing.Guide.Forms.Commands {
   public interface ICommandFactory {
      TCommand GetCommand<TCommand>() where TCommand : IAsyncCommand;
   }
}