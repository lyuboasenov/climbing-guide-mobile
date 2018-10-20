using System.Collections.Generic;

namespace Climbing.Guide.Core.Models.Routes {
   public interface IGradeService {
      IGrade GetGrade(int absoluteValue, GradeType gradeType);
      IEnumerable<IGrade> GetGradeList(GradeType gradeType);
   }
}