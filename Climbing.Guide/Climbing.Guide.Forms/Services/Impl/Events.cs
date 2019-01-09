using Prism.Events;

namespace Climbing.Guide.Forms.Services.Impl {
   public class Events : Services.Events {

      private IEventAggregator EventAggregator { get; set; }

      public Events(IEventAggregator eventAggregator) {
         EventAggregator = eventAggregator;
      }

      public TEventType GetEvent<TEventType>() where TEventType : Guide.Forms.Events.EventBase, new() {
         return EventAggregator.GetEvent<TEventType>();
      }

      public TEventType GetEvent<TEventType, TPayload>() where TEventType : Guide.Forms.Events.EventBase<TPayload>, new() {
         return EventAggregator.GetEvent<TEventType>();
      }
   }
}
