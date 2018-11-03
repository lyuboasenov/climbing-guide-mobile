using System;
using System.Threading.Tasks;
using Climbing.Guide.Api.Schemas;

namespace Climbing.Guide.Mobile.Forms.Services {
   public interface IErrorService {
      Task HandleExceptionAsync(Exception ex, string errorMessageFormat, params object[] errorMessageParams);
      Task HandleExceptionDetailedAsync(Exception ex, string errorMessage, string detailedErrorMessageFormat, params object[] detailedErrorMessageParams);
      Task HandleApiCallExceptionAsync(ApiCallException ex);
      Task HandleApiCallExceptionAsync(ApiCallException ex, string errorMessage, string detailedErrorMessageFormat);
   }
}