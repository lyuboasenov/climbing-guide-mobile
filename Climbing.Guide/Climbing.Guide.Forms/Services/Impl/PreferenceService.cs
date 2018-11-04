﻿using Xamarin.Essentials;

namespace Climbing.Guide.Forms.Services {
   public class PreferenceService : IPreferenceService {

      public int BoulderingGradeSystem {
         get { return Get(GetSettingKey("system/grades/bouldering"), 2); }
         set { Set(GetSettingKey("system/grades/bouldering"), value); }
      }
      public int SportRouteGradeSystem {
         get { return Get(GetSettingKey("system/grades/sport"), 1); }
         set { Set(GetSettingKey("system/grades/sport"), value); }
      }
      public int TradRouteGradeSystem {
         get { return Get(GetSettingKey("system/grades/trad"), 4); }
         set { Set(GetSettingKey("system/grades/trad"), value); }
      }
      public string LanguageCode {
         get { return Get(GetSettingKey("system/language"), "en"); }
         set { Set(GetSettingKey("system/language"), value); }
      }

      public void Set<T>(string key, T value) {
         Preferences.Set(key, value.ToString());
      }

      public T Get<T>(string key, T defaultValue = default(T)) {
         object value = Preferences.Get(key, defaultValue.ToString());
         return (T)value;
      }

      private string GetSettingKey(string key) {
         return Helpers.UriHelper.Get(Helpers.UriHelper.Schema.set, key).ToString();
      }
   }
}
