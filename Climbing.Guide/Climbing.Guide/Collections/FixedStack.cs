using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Climbing.Guide.Collections {
   public class FixedStack<T> : IEnumerable<T>, IReadOnlyCollection<T>, ICollection {

      [NonSerialized]
      private object _syncRoot;

      private LinkedList<T> InternalList { get; set; } = new LinkedList<T>();

      public int Count { get { return InternalList.Count; } }
      private readonly object fixedStackLock = new object();

      public bool IsSynchronized {
         get {
            return false;
         }
      }

      public object SyncRoot {
         get {
            if (_syncRoot == null) {
               Interlocked.CompareExchange<object>(ref _syncRoot, new object(), (object)null);
            }
            return _syncRoot;
         }
      }

      public int FixedCapacity { get; private set; }

      public FixedStack(int fixedCapacity) {
         if(fixedCapacity <= 0) {
            throw new ArgumentOutOfRangeException(nameof(fixedCapacity));
         }

         FixedCapacity = fixedCapacity;
      }

      public void CopyTo(Array array, int index) {

         if (null == array) {
            throw new ArgumentNullException(nameof(array));
         }

         if (array.Length < Count) {
            throw new ArgumentOutOfRangeException(nameof(array.Length));
         }

         for(int i = Count - 1; i > 0; i--) {
            array.SetValue(InternalList.ElementAt(i), Count - 1 - i);
         }
      }

      public IEnumerator<T> GetEnumerator() {
         throw new NotImplementedException();
      }

      IEnumerator IEnumerable.GetEnumerator() {
         return GetEnumerator();
      }

      public void Clear() {
         lock (fixedStackLock) {
            InternalList.Clear();
         }
      }

      public T Peek() {
         if (Count == 0) {
            throw new ArgumentOutOfRangeException(nameof(Count));
         }

         return InternalList.Last.Value;
      }

      public T Pop() {
         if (Count == 0) {
            throw new ArgumentOutOfRangeException(nameof(Count));
         }

         T item = default(T);
         lock (fixedStackLock) {
            item = InternalList.Last.Value;
            InternalList.RemoveLast();
         }

         return item;
      }

      public void Push(T item) {
         lock (fixedStackLock) {
            InternalList.AddLast(item);
            while (FixedCapacity < Count) {
               InternalList.RemoveFirst();
            }
         }
      }
   }
}
