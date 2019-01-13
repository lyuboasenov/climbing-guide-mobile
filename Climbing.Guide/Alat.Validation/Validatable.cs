namespace Alat.Validation {
   public interface Validatable {
      ValidationContext ValidationContext { get; }
      void OnValidationChanged();
   }
}
