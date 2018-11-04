using Climbing.Guide.Forms.Events;

namespace Climbing.Guide.Forms.Services {
   public interface IEventService {
      TEventType GetEvent<TEventType>() where TEventType : EventBase, new();
      TEventType GetEvent<TEventType, TPayload>() where TEventType : EventBase<TPayload>, new();
   }
}