using Climbing.Guide.Api.Schemas;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services {
   public interface IResourceService {
      Task<ObservableCollection<Language>> GetLanguagesAsync(bool force = false);
      Task<ObservableCollection<GradeSystemList>> GetGradeSystemsAsync(bool force = false);
      Task<ObservableCollection<Grade>> GetGradeSystemAsync(int gradeSystemId, bool force = false);
      Task<ObservableCollection<Region>> GetRegionsAsync();
   }
}
