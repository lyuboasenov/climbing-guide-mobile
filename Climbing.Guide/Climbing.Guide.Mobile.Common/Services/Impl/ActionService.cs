using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Climbing.Guide.Mobile.Common.Services {
   public class ActionService : IActionService {
      private Dictionary<Uri, Action> Actions { get; } = new Dictionary<Uri, Action>();

      public void Register(string uri, Action action) {
         Register(new Uri(uri), action);
      }

      public void Register(Uri uri, Action action) {
         Actions[uri] = action;
      }

      public async Task InvokeAsync(string uri) {
         await InvokeAsync(new Uri(uri));
      }

      public async Task InvokeAsync(Uri uri) {
         if (!Actions.ContainsKey(uri)) {
            throw new ArgumentException(Resources.Strings.Main.Action_Service_Action_Not_Found);
         }

         await Task.Run(Actions[uri]);
      }

      public void Invoke(string uri) {
         Invoke(new Uri(uri));
      }

      public void Invoke(Uri uri) {
         if (!Actions.ContainsKey(uri)) {
            throw new ArgumentException(Resources.Strings.Main.Action_Service_Action_Not_Found);
         }

         Actions[uri].Invoke();
      }
   }
}
