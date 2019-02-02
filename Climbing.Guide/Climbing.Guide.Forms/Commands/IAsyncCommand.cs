using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Commands {
   public interface IAsyncCommand {
      Task ExecuteAsync();
   }
}
