using Prism.Events;

namespace Climbing.Guide.Mobile.Common.Services {
   public class EventService : IEventService {

      private IEventAggregator EventAggregator { get; set; }

      public EventService(IEventAggregator eventAggregator) {
         EventAggregator = eventAggregator;
      }

      public TEventType GetEvent<TEventType>() where TEventType : EventBase, new() {
         return EventAggregator.GetEvent<TEventType>();
      }
   }
}
