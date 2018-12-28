namespace Climbing.Guide.Forms.Services {
   public interface IPreferences {
      int BoulderingGradeSystem { get; set; }
      string LanguageCode { get; set; }
      int SportRouteGradeSystem { get; set; }
      int TradRouteGradeSystem { get; set; }

      T Get<T>(string key, T defaultValue = default(T));
      void Set<T>(string key, T value);
   }
}