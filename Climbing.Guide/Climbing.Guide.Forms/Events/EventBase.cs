using Prism.Events;

namespace Climbing.Guide.Forms.Events {
   public class EventBase : PubSubEvent {
   }

   public class EventBase<T> : PubSubEvent<T> {

   }
}
