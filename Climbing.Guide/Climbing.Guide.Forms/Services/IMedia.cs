using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services {
   public interface IMedia {

      /// <summary>
      /// Gets if ability to take photos supported on the device
      /// </summary>
      bool IsTakePhotoSupported { get; }

      /// <summary>
      /// Gets if the ability to pick photo is supported on the device
      /// </summary>
      bool IsPickPhotoSupported { get; }

      /// <summary>
      /// Picks a photo from the default gallery
      /// </summary>
      /// <param name="options">Pick Photo Media Options</param>
      /// <returns>Media file or null if canceled</returns>
      Task<string> PickPhotoAsync();

      /// <summary>
      /// Take a photo async with specified options
      /// </summary>
      /// <param name="options">Camera Media Options</param>
      /// <returns>Media file of photo or null if canceled</returns>
      Task<string> TakePhotoAsync();
   }
}
