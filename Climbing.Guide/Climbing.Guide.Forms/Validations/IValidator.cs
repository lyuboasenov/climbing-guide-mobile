namespace Climbing.Guide.Forms.Validations {
   public interface IValidator {
      void Validate(IValidatable target, string key, object value);
   }
}
