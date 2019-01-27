using Climbing.Guide.Api.Schemas;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services {
   public interface IResource {
      Task<IEnumerable<Language>> GetLanguagesAsync(bool force = false);
      Task<IEnumerable<GradeSystemList>> GetGradeSystemsAsync(bool force = false);
      Task<IEnumerable<Grade>> GetGradeSystemAsync(int gradeSystemId, bool force = false);
   }
}
