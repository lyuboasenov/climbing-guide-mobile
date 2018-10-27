﻿using Prism.Services;
using System.Threading.Tasks;

namespace Climbing.Guide.Mobile.Forms.Services {
   public class AlertService : IAlertService {

      private IPageDialogService InternalPageDialogService { get; set; }

      public AlertService(IPageDialogService pageDialogService) {
         InternalPageDialogService = pageDialogService;
      }

      public async Task<string> DisplayActionSheetAsync(string title, string cancel, string destruction, params string[] buttons) {
         return await InternalPageDialogService.DisplayActionSheetAsync(title, cancel, destruction, buttons);
      }

      public async Task DisplayAlertAsync(string title, string message, string cancel) {
         await InternalPageDialogService.DisplayAlertAsync(title, message, cancel);
      }

      public async Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel) {
         return await InternalPageDialogService.DisplayAlertAsync(title, message, accept, cancel);
      }
   }
}
