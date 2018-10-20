namespace Climbing.Guide.Core.Models.Routes {
   public interface IGrade {
      int AbsoluteValue { get; }
      string Name { get; set; }
   }
}