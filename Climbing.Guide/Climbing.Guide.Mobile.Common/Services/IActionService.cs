using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Mobile.Common.Services {
   public interface IActionService {
      void Invoke(string uri);
      void Invoke(Uri uri);
      Task InvokeAsync(string uri);
      Task InvokeAsync(Uri uri);
      void Register(string uri, Action action);
      void Register(Uri uri, Action action);
   }
}