using Plugin.Media.Abstractions;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services {
   public class Media : IMedia {
      private Plugin.Media.Abstractions.IMedia XamarinMedia { get; set; }

      public bool IsTakePhotoSupported {
         get {
            return XamarinMedia.IsCameraAvailable && XamarinMedia.IsTakePhotoSupported;
         }
      }

      public bool IsPickPhotoSupported {
         get {
            return XamarinMedia.IsCameraAvailable && XamarinMedia.IsPickPhotoSupported;
         }
      }

      public Media(Plugin.Media.Abstractions.IMedia media) {
         XamarinMedia = media;
      }

      public async Task<string> PickPhotoAsync() {
         return (await XamarinMedia.PickPhotoAsync()).Path;
      }

      public async Task<string> TakePhotoAsync() {
         StoreCameraMediaOptions options = new StoreCameraMediaOptions();
         return (await XamarinMedia.TakePhotoAsync(options)).Path;
      }
   }
}
