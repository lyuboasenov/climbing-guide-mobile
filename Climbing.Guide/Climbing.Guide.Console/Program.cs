using Climbing.Guide.Api;
using Climbing.Guide.Core.Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Climbing.Guide.Console {
   class Program {
      static void Main(string[] args) {
         string choice = "";
         do {
            System.Console.Clear();
            System.Console.WriteLine("Climbing guide tester:");
            System.Console.WriteLine("   1. TestRestApiCalls");
            System.Console.WriteLine("   2. TestRestApiLogin");
            System.Console.WriteLine("   3. Insert Regions");
            System.Console.WriteLine("   4. Insert Area / Routes");


            choice = System.Console.ReadLine().Trim();
            switch (choice) {
               case "1":
                  TestRestApiCalls();
                  break;
               case "2":
                  TestRestApiLogin();
                  break;
               case "3":
                  InsertRegions();
                  break;
               case "4":
                  InsertRoutes();
                  break;
            }

         } while (string.CompareOrdinal(choice, "q") != 0);
      }

      // todo:
      private static void InsertRoutes() {
         var serializer = JsonSerializer.Create(new JsonSerializerSettings() {
            Formatting = Formatting.None,
            TypeNameHandling = TypeNameHandling.None
         });

         var client = GetRestApiClient();
         if (Login(client)) {
            Region[] regions = null;
            using (var streamReader = new StreamReader(@"D:\_projects\climbing-guide-api\8a.nu.routes.output.csv"))
            using (var reader = new JsonTextReader(streamReader)) {
               regions = serializer.Deserialize<Region[]>(reader);
            }
            var countries = regions.Select(r => r.Country).Distinct();
            int counter = 0;
            int globalRegionId = 5970;

            Dictionary<int, Region> distinctRegions = new Dictionary<int, Region>();
            foreach (var region in regions) {
               if (!distinctRegions.ContainsKey(region.Id)) {
                  distinctRegions.Add(region.Id, region);
               } else {
                  distinctRegions[region.Id].Areas.AddRange(region.Areas);
               }
            }

            regions = distinctRegions.Values.ToArray();

            foreach (var region in regions) {
               region.Areas = ReMapAreas(region.Areas);
            }

            //List<Route> routes = new List<Route>();

            //foreach (var region in regions) {
            //   foreach (var area in region.Areas) {
            //      routes.AddRange(area.Routes);
            //   }
            //}

            //var distinctGrades = routes.Select(r => new { Type = r.Type, Grade = r.Grade }).Distinct();

            //foreach(var grade in distinctGrades) {
            //   System.Console.WriteLine($"{grade.Type} / {grade.Grade}");
            //}

            //return;

            foreach (var region in regions) {
               Api.Schemas.Area payload = new Api.Schemas.Area() {
                  Name = region.Name,
                  Info = string.IsNullOrEmpty(region.Info) ? "tba" : region.Info,
                  Latitude = region.Position[0],
                  Longitude = region.Position[1],
                  Size = 20,
                  Restrictions = region.Access,
                  Region_id = globalRegionId,
                  Tags = $"{GetCountryCode(region.Country)},{region.Country},{region.City},{region.Ascents}"
               };

               Api.Schemas.Area createdArea = client.AreasClient.CreateAsync(payload, globalRegionId).Result;

               foreach (var area in region.Areas) {
                  Api.Schemas.Sector sectorPayload = new Api.Schemas.Sector() {
                     Area_id = createdArea.Id.Value,
                     Name = string.IsNullOrEmpty(area.Name) ? "tba" : area.Name,
                     Info = "tba",
                     Latitude = region.Position[0],
                     Longitude = region.Position[1],
                     Size = 5
                  };

                  Api.Schemas.Sector createdSector = client.SectorsClient.CreateAsync(sectorPayload, createdArea.Id.Value).Result;

                  foreach (var route in area.Routes) {
                     Api.Schemas.Route routePayload = new Api.Schemas.Route() {
                        Sector_id = createdSector.Id.Value,
                        Type = route.Type == 0 ? Api.Schemas.RouteType._2 : Api.Schemas.RouteType._1,
                        Name = string.IsNullOrEmpty(route.Name) ? "tba" : route.Name,
                        Difficulty = GetDifficulty(route.Type, route.Grade),
                        Info = "tba"
                     };

                     client.RoutesClient.CreateAsync(routePayload, createdSector.Id.Value).Wait();
                  }
               }


               System.Console.WriteLine($"{counter++}/{regions.Length}: {region} added sucessfully.");
            }
         }
      }

      private static List<Area> ReMapAreas(List<Area> areas) {
         Dictionary<string, Area> remappedAreas = new Dictionary<string, Area>();
         List<Route> routes = new List<Route>();


         foreach (var area in areas) {
            string areaName = GetAreaName(area);

            if (!remappedAreas.ContainsKey(areaName)) {
               remappedAreas.Add(areaName, new Area() { Name = areaName, Routes = new List<Route>() });
            }

            foreach (var route in area.Routes) {
               if (!routes.Contains(route, routeComparer)) {
                  routes.Add(route);
               }
            }
         }

         foreach (var route in routes) {
            string areaName = GetAreaName(route);
            remappedAreas[areaName].Routes.Add(route);
         }

         return remappedAreas.Values.ToList();
      }

      private static RouteComparer routeComparer = new RouteComparer();

      private class RouteComparer : IEqualityComparer<Route> {
         public bool Equals(Route x, Route y) {
            return x.Grade == y.Grade && x.Type == x.Type && x.Name == y.Name;
         }

         public int GetHashCode(Route obj) {
            return obj.GetHashCode();
         }
      }

      private static string GetAreaName(Route route) {
         var result = route.Area;
         if (result.Contains("/")) {
            result = result.Split('/')[1].Trim();
         } else if (result.Contains(">")) {
            result = result.Split('>')[1].Trim();
         } else if (string.IsNullOrEmpty(result)) {
            result = "default";
         }

         return result;
      }

      private static string GetAreaName(Area area) {
         string result = "default";
         if (area.Routes.Count > 0) {
            result = GetAreaName(area.Routes[0]);
         }

         return result;
      }

      private static Dictionary<string, int> routeGrades = new Dictionary<string, int>() {
         { "2", 10 },
         { "3a", 20 },
         { "3a+", 25 },
         { "3b", 30 },
         { "3b+", 35 },
         { "3c", 40 },
         { "3c+", 45 },
         { "4a", 50 },
         { "4a+", 55 },
         { "4b", 60 },
         { "4b+", 65 },
         { "4c", 70 },
         { "4c+", 75 },
         { "5a", 80 },
         { "5a+", 85 },
         { "5b", 90 },
         { "5b+", 95 },
         { "5c", 100 },
         { "5c+", 105 },
         { "6a", 110 },
         { "6a+", 115 },
         { "6b", 120 },
         { "6b+", 125 },
         { "6c", 130 },
         { "6c+", 135 },
         { "7a", 140 },
         { "7a+", 145 },
         { "7b", 150 },
         { "7b+", 155 },
         { "7c", 160 },
         { "7c+", 165 },
         { "8a", 170 },
         { "8a+", 175 },
         { "8b", 180 },
         { "8b+", 185 },
         { "8c", 190 },
         { "8c/+", 192 },
         { "8c+", 195 },
         { "8c+/9a", 197 },
         { "9a", 200 },
         { "9a+", 205 },
         { "9b", 210 },
         { "9b+", 215 },
         { "9c", 220 },
         { "9c+", 225 }
      };
      private static Dictionary<string, int> boulderGrades = new Dictionary<string, int>() {
         { "2", 10 },
         { "3A", 20 },
         { "3B", 22 },
         { "3C", 24 },
         { "4A", 25 },
         { "4B", 30 },
         { "4C", 35 },
         { "5-", 40 },
         { "5A", 45 },
         { "5B", 50 },
         { "5C", 50 },
         { "6A", 55 },
         { "6A+", 60 },
         { "6B", 65 },
         { "6B+", 70 },
         { "6C", 75 },
         { "6C+", 80 },
         { "7A", 85 },
         { "7A+", 90 },
         { "7B", 95 },
         { "7B+", 100 },
         { "7C", 105 },
         { "7C+", 110 },
         { "8A", 115 },
         { "8A+", 120 },
         { "8B", 125 },
         { "8B+", 130 },
         { "8C", 135 },
         { "8C+", 140 },
         { "9A", 145 }
      };

      private static int GetDifficulty(int type, string grade) {
         var grades = type == 0 ? routeGrades : boulderGrades;
         return grades[type == 0 ? grade.ToLower() : grade.ToUpper()];
      }

      private static void InsertRegions() {
         //var serializer = JsonSerializer.Create(new JsonSerializerSettings() {
         //   Formatting = Formatting.None,
         //   TypeNameHandling = TypeNameHandling.None
         //});

         //var client = GetRestApiClient();
         //if (Login(client)) {
         //   Region[] regions = null;
         //   using (var streamReader = new StreamReader(@"D:\_projects\climbing-guide-api\8a.nu.regions.output.csv"))
         //   using (var reader = new JsonTextReader(streamReader)) {
         //      regions = serializer.Deserialize<Region[]>(reader);
         //   }
         //   var countries = regions.Select(r => r.Country).Distinct();
         //   int counter = 0;
         //   foreach (var region in regions) {
         //      Api.Schemas.Region payload = new Api.Schemas.Region() {
         //         Name = region.Name,
         //         Info = string.IsNullOrEmpty(region.Info) ? "tba" : region.Info,
         //         Latitude = region.Position[0],
         //         Longitude = region.Position[1],
         //         Size = 20,
         //         Restrictions = region.Access,
         //         Country_code = GetCountryCode(region.Country),
         //         Tags = $"{region.Country},{region.City},{region.Ascents}"
         //      };

         //      client.RegionsClient.CreateAsync(payload).Wait();
         //      System.Console.WriteLine($"{counter++}/{regions.Length}: {region} added sucessfully.");
         //   }
         //}

         var client = GetRestApiClient();
         if (Login(client)) {
            do {
               try {
                  Api.Schemas.Region payload = new Api.Schemas.Region() {
                     Name = "Test region name",
                     Info = "Test region information",
                     Latitude = 42.42m,
                     Longitude = 23.23m,
                     Size = 20,
                     Restrictions = "Test region restrictions",
                     Country_code = "uk",
                     Tags = $"climbing, sick"
                  };

                  var createdRegion = client.RegionsClient.CreateAsync(payload).Result;
                  System.Console.WriteLine($"Region added with id {createdRegion.Id} sucessfully.");
               } catch (Exception ex) {
                  System.Console.WriteLine($"Error occured: {ex.Message}.");
                  System.Console.Write($"Retry (y/n):");
                  var answer = System.Console.ReadKey();
                  if (answer.KeyChar != 'y' && answer.KeyChar != 'Y') {
                     break;
                  }
               }
            } while (true);
         }
      }
      
      private static Dictionary<string, string> CoutryCodes { get; set; }
      private static string GetCountryCode(string country) {
         if (null == CoutryCodes) {
            CoutryCodes = new Dictionary<string, string>();
            var lines = File.ReadAllLines(@"D:\_projects\climbing-guide-api\coutry.codes.csv");
            foreach(var line in lines) {
               var parts = line.Split(',');
               CoutryCodes.Add(parts[0], parts[1]);
            }
         }

         if (!CoutryCodes.ContainsKey(country)) {
            throw new ArgumentException($"Country: {country} not found.");
         }

         return CoutryCodes[country];
      }

      private static bool Login(IApiClient client) {
         System.Console.Write("username:");
         var username = System.Console.ReadLine();
         System.Console.Write("password:");
         var password = System.Console.ReadLine();

         bool success = client.AuthenticationManager.LoginAsync(username, password).Result;
         if (success) {
            System.Console.WriteLine("Success!");
         } else {
            System.Console.Write("Failure!");
         }

         return success;
      }

      private static void TestRestApiLogin() {
         string choice = "";
         var client = GetRestApiClient();
         do {
            System.Console.ReadLine();
            Login(client);

            System.Console.Write("Retry (y/n):");
            choice = System.Console.ReadLine();
         } while (string.CompareOrdinal(choice, "n") != 0);
         System.Console.ReadLine();
      }

      private static void TestRestApiCalls() {
         var client = GetRestApiClient();
         var regions = client.RegionsClient.ListAsync().Result.Results;

         System.Console.Clear();
         foreach(var region in regions) {
            System.Console.WriteLine("Region: {0} / {1}", region.Id, region.Name);
         }

         System.Console.ReadLine();
      }

      private static IApiClient GetRestApiClient() {
         var httpClient = new System.Net.Http.HttpClient() { BaseAddress = new Uri("http://127.0.0.1:8000/") };
         var clientId = "ZPmM58IFJm2XljO0IaNVTCvRZFZtbu8tJuPHjLRS";
         var clientSecret = "bDZ6mLIkKQmS2elBkie9QUsWSMQGzJjmoqiTQKwq0QxdVfqujLLkENDBZhfgXxvCxUMxxIzfZgoCYyunYQV2n4sQsgVBzAjjOMed7OgNmuAD6R6zG5b3LH9F7bXtnlfW";

         return new ApiClient(new ApiClientSettings(
            httpClient,
            new OAuthAuthenticatonManager(httpClient, clientId, clientSecret)));
      }
   }
}
