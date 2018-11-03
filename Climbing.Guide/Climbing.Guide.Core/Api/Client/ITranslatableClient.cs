using Climbing.Guide.Api.Schemas;

namespace Climbing.Guide.Api.Client {
   public interface ITranslatableClient {
      Language SelectedLanguage { get; set; }
   }
}
