using Climbing.Guide.Core.API;
using System;

namespace Climbing.Guide.Console {
   class Program {
      static void Main(string[] args) {
         string choice = "";
         do {
            System.Console.Clear();
            System.Console.WriteLine("Climbing guide tester:");
            System.Console.WriteLine("   1. TestRestApiCalls");
            System.Console.WriteLine("   2. TestRestApiLogin");
            


            choice = System.Console.ReadLine().Trim();
            switch (choice) {
               case "1":
                  TestRestApiCalls();
                  break;
               case "2":
                  TestRestApiLogin();
                  break;
            }

         } while (string.CompareOrdinal(choice, "q") != 0);
      }

      private static void TestRestApiLogin() {
         string choice = "";
         var client = GetRestApiClient();
         do {
            System.Console.ReadLine();
            System.Console.Write("username:");
            var username = System.Console.ReadLine();
            System.Console.Write("password:");
            var password = System.Console.ReadLine();

            if(client.LoginAsync(username, password).Result) {
               System.Console.WriteLine("Success!");
            } else {
               System.Console.Write("Failure!");
            }

            System.Console.Write("Retry (y/n):");
            choice = System.Console.ReadLine();
         } while (string.CompareOrdinal(choice, "n") != 0);
         System.Console.ReadLine();
      }

      private static void TestRestApiCalls() {
         var client = GetRestApiClient();
         var regions = client.RegionsClient.ListAsync().Result;

         System.Console.Clear();
         foreach(var region in regions) {
            System.Console.WriteLine("Region: {0} / {1}", region.Id, region.Name);
         }

         System.Console.ReadLine();
      }

      private static IRestApiClient GetRestApiClient() {
         return new RestApiClient();
      }
   }
}
