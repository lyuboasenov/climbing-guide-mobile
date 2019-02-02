namespace Climbing.Guide.Forms.Commands {
   public interface ICommandQueryFactory {
      TCommand GetCommand<TCommand>() where TCommand : IAsyncCommand;
      TQuery GetQuery<TQuery>() where TQuery : IAsyncQuery;
   }
}