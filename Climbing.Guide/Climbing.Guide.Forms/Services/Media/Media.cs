using Plugin.Media.Abstractions;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services.Media {
   public class Media : IMedia {
      private Plugin.Media.Abstractions.IMedia XamarinMedia { get; }

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
         PickMediaOptions options = new PickMediaOptions() {
            CompressionQuality = 70,
            RotateImage = true,
            SaveMetaData = true,
            MaxWidthHeight = 2048
         };
         var response = await XamarinMedia.PickPhotoAsync(options);
         return response.Path;
      }

      public async Task<string> TakePhotoAsync() {
         StoreCameraMediaOptions options = new StoreCameraMediaOptions();
         return (await XamarinMedia.TakePhotoAsync(options)).Path;
      }
   }
}
