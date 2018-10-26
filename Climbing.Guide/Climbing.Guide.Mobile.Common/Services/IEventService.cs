using Prism.Events;

namespace Climbing.Guide.Mobile.Common.Services {
   public interface IEventService {
      TEventType GetEvent<TEventType>() where TEventType : EventBase, new();
   }
}