using System;
using System.Threading.Tasks;
using Climbing.Guide.Core.API.Schemas;

namespace Climbing.Guide.Mobile.Forms.Services {
   public interface IErrorService {
      Task HandleExceptionAsync(Exception ex, string errorMessageFormat, params object[] errorMessageParams);
      Task HandleExceptionDetailedAsync(Exception ex, string errorMessage, string detailedErrorMessageFormat, params object[] detailedErrorMessageParams);
      Task HandleRestApiCallExceptionAsync(RestApiCallException ex);
      Task HandleRestApiCallExceptionAsync(RestApiCallException ex, string errorMessage, string detailedErrorMessageFormat);
   }
}