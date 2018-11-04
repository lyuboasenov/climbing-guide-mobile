using Plugin.Media.Abstractions;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services {
   public class MediaService : IMediaService {
      private IMedia Media { get; set; }

      public bool IsTakePhotoSupported {
         get {
            return Media.IsCameraAvailable && Media.IsTakePhotoSupported;
         }
      }

      public bool IsPickPhotoSupported {
         get {
            return Media.IsCameraAvailable && Media.IsPickPhotoSupported;
         }
      }

      public MediaService(IMedia media) {
         Media = media;
      }

      public async Task<string> PickPhotoAsync() {
         return (await Media.PickPhotoAsync()).Path;
      }

      public async Task<string> TakePhotoAsync() {
         StoreCameraMediaOptions options = new StoreCameraMediaOptions();
         return (await Media.TakePhotoAsync(options)).Path;
      }
   }
}
