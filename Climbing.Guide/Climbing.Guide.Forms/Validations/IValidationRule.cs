namespace Climbing.Guide.Forms.Validations {
   public interface IValidationRule {
      string ErrorMessage { get; }
      bool IsValid { get; }
      void Validate(string key, object value);
   }
}
