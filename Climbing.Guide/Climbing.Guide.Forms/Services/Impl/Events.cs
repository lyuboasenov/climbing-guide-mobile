using Prism.Events;

namespace Climbing.Guide.Forms.Services {
   public class Events : IEvents {

      private IEventAggregator EventAggregator { get; set; }

      public Events(IEventAggregator eventAggregator) {
         EventAggregator = eventAggregator;
      }

      public TEventType GetEvent<TEventType>() where TEventType : Forms.Events.EventBase, new() {
         return EventAggregator.GetEvent<TEventType>();
      }

      public TEventType GetEvent<TEventType, TPayload>() where TEventType : Forms.Events.EventBase<TPayload>, new() {
         return EventAggregator.GetEvent<TEventType>();
      }
   }
}
