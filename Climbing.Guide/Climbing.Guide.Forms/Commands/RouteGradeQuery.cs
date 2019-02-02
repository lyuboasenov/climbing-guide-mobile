using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Forms.Commands.Generics;
using Climbing.Guide.Forms.Services.Preferences;
using Climbing.Guide.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Commands {
   public class RouteGradeQuery : IAsyncQuery<Grade>, IQuery<Grade> {
      public Route Route { get; set; }

      private ISyncTaskRunner SyncTaskRunner { get; }
      private ICommandQueryFactory CommandQueryFactory { get; }
      private IPreferences Preferences { get; }

      public RouteGradeQuery(ICommandQueryFactory commandQueryFactory,
         ISyncTaskRunner syncTaskRunner,
         IPreferences preferences) {
         SyncTaskRunner = syncTaskRunner ?? throw new ArgumentNullException(nameof(syncTaskRunner));
         CommandQueryFactory = commandQueryFactory ?? throw new ArgumentNullException(nameof(commandQueryFactory));
         Preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
      }

      public Grade GetResult() {
         return SyncTaskRunner.RunSync(GetResultAsync);
      }

      public async Task<Grade> GetResultAsync() {
         VerifyInputParameters();

         var query = CommandQueryFactory.GetQuery<GradeSystemQuery>();
         query.GradeSystemId = GetGradeSystemId(Route);

         var grades = await query.GetResultAsync();
         var grade = grades.Where(g => g.Value <= Route.Difficulty).OrderByDescending(g => g.Value).FirstOrDefault();

         return grade ?? throw new KeyNotFoundException($"Grade not found for route {Route.Name} with id {Route.Id}");
      }

      object IQuery.GetResult() {
         return GetResult();
      }

      async Task<object> IAsyncQuery.GetResultAsync() {
         return await GetResultAsync();
      }

      private int GetGradeSystemId(Route route) {
         var gradeSystemId = 1;
         var routeType = route.Type;

#pragma warning disable RCS1179 // Use return instead of assignment.
         if (routeType == RouteType._1) {
            gradeSystemId = Preferences.BoulderingGradeSystem;
         } else if (routeType == RouteType._2) {
            gradeSystemId = Preferences.SportRouteGradeSystem;
         } else if (routeType == RouteType._4) {
            gradeSystemId = Preferences.TradRouteGradeSystem;
         }
#pragma warning restore RCS1179 // Use return instead of assignment.

         return gradeSystemId;
      }

      private void VerifyInputParameters() {
         if (Route == null) {
#pragma warning disable S3928 // Parameter names used into ArgumentException constructors should match an existing one 
            throw new ArgumentNullException(nameof(Route));
#pragma warning restore S3928 // Parameter names used into ArgumentException constructors should match an existing one 
         }
      }
   }
}
