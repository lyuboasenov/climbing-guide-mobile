using Prism.Navigation;
using System;
using System.Collections.Generic;

namespace Climbing.Guide.Mobile.Common.Services {
   public interface INavigationService : Prism.Navigation.INavigationService {
      Uri GetNavigationUri(string absolutePath);
      Uri GetShellNavigationUri(string relativePath);
      INavigationParameters GetParameters(params KeyValuePair<string, object>[] parameters);
   }
}