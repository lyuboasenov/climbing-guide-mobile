using System.Linq;

namespace Climbing.Guide.Collections.ObjectModel {
   /// <summary>
   /// ObservableCollection throws ArgumentOutOfRangeException on Clear
   /// https://stackoverflow.com/questions/47347273/clear-observablecollection-throws-exception
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public class ObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T> {
      /// <summary>
      /// Normal ObservableCollection fails if you are trying to clear ObservableCollection<T> if there is data inside
      /// this is workaround till it will be fixed in Xamarin Forms
      /// </summary>
      protected override void ClearItems() {
         while (Items.Any()) {
            Items.RemoveAt(0);
         }
      }
   }
}
