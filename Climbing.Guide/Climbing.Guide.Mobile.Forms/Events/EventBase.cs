using Prism.Events;

namespace Climbing.Guide.Mobile.Forms.Events {
   public class EventBase : PubSubEvent {
   }

   public class EventBase<T> : PubSubEvent<T> {

   }
}
