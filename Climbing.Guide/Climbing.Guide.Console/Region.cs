using System;
using System.Collections.Generic;
using System.Text;

namespace Climbing.Guide.Console {
   public class Region {
      public int Ascents { get; set; }
      public int Id { get; set; }
      public string Name { get; set; }
      public decimal[] Position { get; set; }
      public string Info { get; set; }
      public string Access { get; set; }
      public string Country { get; set; }
      public string City { get; set; }

      public override string ToString() {
         return $"{Id} {Name}";
      }
   }
}
