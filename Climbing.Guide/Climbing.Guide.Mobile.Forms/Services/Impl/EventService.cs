using Prism.Events;

namespace Climbing.Guide.Mobile.Forms.Services {
   public class EventService : IEventService {

      private IEventAggregator EventAggregator { get; set; }

      public EventService(IEventAggregator eventAggregator) {
         EventAggregator = eventAggregator;
      }

      public TEventType GetEvent<TEventType>() where TEventType : Events.EventBase, new() {
         return EventAggregator.GetEvent<TEventType>();
      }

      public TEventType GetEvent<TEventType, TPayload>() where TEventType : Events.EventBase<TPayload>, new() {
         return EventAggregator.GetEvent<TEventType>();
      }
   }
}
