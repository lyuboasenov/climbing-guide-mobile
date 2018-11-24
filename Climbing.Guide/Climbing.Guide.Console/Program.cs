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
            using (var streamReader = new StreamReader(@"D:\_projects\climbing-guide-api\8a.nu.regions.output.csv"))
            using (var reader = new JsonTextReader(streamReader)) {
               regions = serializer.Deserialize<Region[]>(reader);
            }
            var countries = regions.Select(r => r.Country).Distinct();
            int counter = 0;
            foreach (var region in regions) {
               Api.Schemas.Region payload = new Api.Schemas.Region() {
                  Name = region.Name,
                  Info = string.IsNullOrEmpty(region.Info) ? "tba" : region.Info,
                  Latitude = region.Position[0],
                  Longitude = region.Position[1],
                  Size = 20,
                  Restrictions = region.Access,
                  Country_code = GetCountryCode(region.Country),
                  Tags = $"{region.Country},{region.City},{region.Ascents}"
               };

               client.RegionsClient.CreateAsync(payload).Wait();
               System.Console.WriteLine($"{counter++}/{regions.Length}: {region} added sucessfully.");
            }
         }
      }

      private static void InsertRegions() {
         var serializer = JsonSerializer.Create(new JsonSerializerSettings() {
            Formatting = Formatting.None,
            TypeNameHandling = TypeNameHandling.None
         });

         var client = GetRestApiClient();
         if (Login(client)) {
            Region[] regions = null;
            using (var streamReader = new StreamReader(@"D:\_projects\climbing-guide-api\8a.nu.regions.output.csv"))
            using (var reader = new JsonTextReader(streamReader)) {
               regions = serializer.Deserialize<Region[]>(reader);
            }
            var countries = regions.Select(r => r.Country).Distinct();
            int counter = 0;
            foreach (var region in regions) {
               Api.Schemas.Region payload = new Api.Schemas.Region() {
                  Name = region.Name,
                  Info = string.IsNullOrEmpty(region.Info) ? "tba" : region.Info,
                  Latitude = region.Position[0],
                  Longitude = region.Position[1],
                  Size = 20,
                  Restrictions = region.Access,
                  Country_code = GetCountryCode(region.Country),
                  Tags = $"{region.Country},{region.City},{region.Ascents}"
               };

               client.RegionsClient.CreateAsync(payload).Wait();
               System.Console.WriteLine($"{counter++}/{regions.Length}: {region} added sucessfully.");
            }
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

         bool success = client.LoginAsync(username, password).Result;
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
         return new ApiClient(new ApiClientSettings("http://127.0.0.1:8000/"));
      }
   }
}
