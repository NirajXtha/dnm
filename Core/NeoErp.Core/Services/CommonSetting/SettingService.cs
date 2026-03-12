using NeoErp.Core.Caching;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace NeoErp.Core.Services.CommonSetting
{
    public partial class SettingService : ISettingService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string SETTINGS_ALL_KEY = "NeoErp.setting.all_{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string SETTINGS_PATTERN_KEY = "NeoErp.setting.";
        private const string filepath = "/App_Data/Json/";

        #endregion

        #region Fields

        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        
        public SettingService(ICacheManager cacheManager)
        {
            this._cacheManager = cacheManager;
          
        }

        #endregion

        #region Nested classes

        [Serializable]
        public class SettingForCaching
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
          
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>Settings</returns>
        protected virtual IDictionary<string, IList<SettingForCaching>> GetAllSettingsCached(string filename= "setting")
        {
            //cache
            string key = string.Format(SETTINGS_ALL_KEY,filename);
            return _cacheManager.Get(key, () =>
            {
                //we use no tracking here for performance optimization
                //anyway records are loaded only for read-only operations
                var dictionary = new Dictionary<string, IList<SettingForCaching>>();
                if (!File.Exists(CommonHelper.MapPath(filepath + string.Format("{0}.json", filename))))
                    return dictionary;
                string json = File.ReadAllText(CommonHelper.MapPath(filepath + string.Format("{0}.json", filename)));
                var pro = JsonConvert.DeserializeObject<List<Setting>>(json);
                var settings = pro.ToList();
              
                foreach (var s in settings)
                {
                    var resourceName = s.Name.ToLowerInvariant();
                    var settingForCaching = new SettingForCaching
                    {
                        Name = s.Name,
                        Value = s.Value,
                    };
                    if (!dictionary.ContainsKey(resourceName))
                    {
                        //first setting
                        dictionary.Add(resourceName, new List<SettingForCaching>
                        {
                            settingForCaching
                        });
                    }
                    else
                    {
                        //already added
                        //most probably it's the setting with the same name but for some certain store (storeId > 0)
                        dictionary[resourceName].Add(settingForCaching);
                    }
                }
                return dictionary;
            });
        }


        #endregion

        #region Methods

        /// <summary>
        /// Adds a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void InsertSetting(Setting setting, bool clearCache = true,string filename="setting")
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            string jsondata = new JavaScriptSerializer().Serialize(setting);
            //  string filename = string.Format("{0}.json", filename);
            // Write that JSON to txt file,  
            var text = new StringBuilder();
            if (File.Exists(CommonHelper.MapPath(filepath + string.Format("{0}.json", filename))))
            {
                foreach (string s in File.ReadAllLines(CommonHelper.MapPath(filepath + string.Format("{0}.json", filename))))
                {
                    text.AppendLine(s.Replace("]", ""));

                }
                string path = CommonHelper.MapPath(filepath + string.Format("{0}.json", filename));
                System.Diagnostics.Debug.WriteLine(path);

                string content = text + "," + jsondata + "]";
                System.Diagnostics.Debug.WriteLine(content);

                System.IO.File.WriteAllText(CommonHelper.MapPath(filepath + string.Format("{0}.json", filename)), text + "," + jsondata + "]");
            }

            else
            {
                string path = CommonHelper.MapPath(filepath + string.Format("{0}.json", filename));
                System.Diagnostics.Debug.WriteLine(path);

                string content = "[" + jsondata + "]";
                System.Diagnostics.Debug.WriteLine(content);
                System.IO.File.WriteAllText(CommonHelper.MapPath(filepath + string.Format("{0}.json", filename)), "[" + jsondata + "]");
            }



            //cache
            if (clearCache)
                _cacheManager.RemoveByPattern(SETTINGS_PATTERN_KEY);

        }

        public virtual bool DeleteSetting(string filename)
        {
            try
            {
                if (!File.Exists(CommonHelper.MapPath(filepath + string.Format("{0}.json", filename))))
                    return true;
                File.Delete(CommonHelper.MapPath(filepath + string.Format("{0}.json", filename)));
                return true;
            }
            catch(Exception ec)
            {
                return false;
            }
            
        }

        /// <summary>
        /// Gets a setting by identifier
        /// </summary>
        /// <param name="settingId">Setting identifier</param>
        /// <returns>Setting</returns>
        public virtual Setting GetSettingByName(string settingName)
        {
            if (string.IsNullOrEmpty(settingName))
                return null;
            string json = File.ReadAllText(CommonHelper.MapPath(filepath + string.Format("{0}.json", "setting")));
            var pro = JsonConvert.DeserializeObject<settinglist>(json);


            var settings = pro.setting.ToList();
            return settings.Where(x => x.Name == settingName).FirstOrDefault();
        }

        /// <summary>
        /// Get setting by key
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>Setting</returns>
        public virtual Setting GetSetting(string key,bool loadSharedValueIfNotFound = false)
        {
            if (String.IsNullOrEmpty(key))
                return null;

            var settings = GetAllSettingsCached();
            key = key.Trim().ToLowerInvariant();
            if (settings.ContainsKey(key))
            {
                var settingsByKey = settings[key];
                var setting = settingsByKey.FirstOrDefault();

                //load shared value?
                if (setting == null && loadSharedValueIfNotFound)
                    setting = settingsByKey.FirstOrDefault();

                if (setting != null)
                    return GetSettingByName(setting.Name);
            }

            return null;
        }



        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>Setting value</returns>
        public virtual T GetSettingByKey<T>(string key, T defaultValue = default(T), bool loadSharedValueIfNotFound = false)
        {
            if (String.IsNullOrEmpty(key))
                return defaultValue;

            var settings = GetAllSettingsCached();
            key = key.Trim().ToLowerInvariant();
            if (settings.ContainsKey(key))
            {
                var settingsByKey = settings[key];
                var setting = settingsByKey.FirstOrDefault();

                //load shared value?
                if (setting == null  && loadSharedValueIfNotFound)
                    setting = settingsByKey.FirstOrDefault();

                if (setting != null)
                    return CommonHelper.To<T>(setting.Value);
            }

            return defaultValue;
        }


        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>Setting value</returns>
        public virtual T GetSettingByKey<T>(string key, T defaultValue = default(T), bool loadSharedValueIfNotFound = false,string filename="testsetting")
        {
            if (String.IsNullOrEmpty(key))
                return defaultValue;

            var settings = GetAllSettingsCached(filename);
            key = key.Trim().ToLowerInvariant();
            if (settings.ContainsKey(key))
            {
                var settingsByKey = settings[key];
                var setting = settingsByKey.FirstOrDefault();

                //load shared value?
                if (setting == null && loadSharedValueIfNotFound)
                    setting = settingsByKey.FirstOrDefault();

                if (setting != null)
                    return CommonHelper.To<T>(setting.Value);
            }

            return defaultValue;
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="storeId">Store identifier for which settigns should be loaded</param>
        public virtual T LoadSetting<T>() where T : ISettings, new()
        {
            var settings = Activator.CreateInstance<T>();

            foreach (var prop in typeof(T).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = typeof(T).Name + "." + prop.Name;
                //load by store
                var setting = GetSettingByKey<string>(key,null,true);
                if (setting == null)
                    continue;

                if (!CommonHelper.GetNeoCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                if (!CommonHelper.GetNeoCustomTypeConverter(prop.PropertyType).IsValid(setting))
                    continue;

                object value = CommonHelper.GetNeoCustomTypeConverter(prop.PropertyType).ConvertFromInvariantString(setting);

                //set property
                prop.SetValue(settings, value, null);
            }

            return settings;
        }
        /// <summary>
        /// Load settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="storeId">Store identifier for which settigns should be loaded</param>
        public virtual T LoadSetting<T>(string filename) where T : ISettings, new()
        {
            var settings = Activator.CreateInstance<T>();

            foreach (var prop in typeof(T).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = typeof(T).Name + "." + prop.Name;
                //load by store
                var setting = GetSettingByKey<string>(key, loadSharedValueIfNotFound: true, filename:filename);
                if (setting == null)
                    continue;

                if (!CommonHelper.GetNeoCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                if (!CommonHelper.GetNeoCustomTypeConverter(prop.PropertyType).IsValid(setting))
                    continue;

                object value = CommonHelper.GetNeoCustomTypeConverter(prop.PropertyType).ConvertFromInvariantString(setting);

                //set property
                prop.SetValue(settings, value, null);
            }

            return settings;
        }
        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void SetSetting<T>(string key, T value, bool clearCache = true)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            key = key.Trim().ToLowerInvariant();
            string valueStr = CommonHelper.GetNeoCustomTypeConverter(typeof(T)).ConvertToInvariantString(value);
            var allSettings = GetAllSettingsCached();
            var settingForCaching = allSettings.ContainsKey(key) ?
                allSettings[key].FirstOrDefault() : null;
            if (settingForCaching != null)
            {
                //update
                //var setting = GetSettingById(settingForCaching.Id);
                //setting.Value = valueStr;
                //UpdateSetting(setting, clearCache);
            }
            else
            {
                //insert
                var setting = new Setting
                {
                    Name = key,
                    Value = valueStr,
                };
                InsertSetting(setting, clearCache);
            }
        }


        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void SetSetting<T>(string key, T value, bool clearCache = true,string filename="testsetting")
        {
            if (key == null)
                throw new ArgumentNullException("key");
            key = key.Trim().ToLowerInvariant();
            string valueStr = CommonHelper.GetNeoCustomTypeConverter(typeof(T)).ConvertToInvariantString(value);
            var allSettings = GetAllSettingsCached(filename);
            var settingForCaching = allSettings.ContainsKey(key) ?
                allSettings[key].FirstOrDefault() : null;
            if (settingForCaching != null)
            {
                //update
                //var setting = GetSettingById(settingForCaching.Id);
                //setting.Value = valueStr;
                //UpdateSetting(setting, clearCache);
            }
            else
            {
                //insert
                var setting = new Setting
                {
                    Name = key,
                    Value = valueStr,
                };
                InsertSetting(setting, clearCache,filename);
            }
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>Settings</returns>
        public virtual IList<Setting> GetAllSettings()
        {
            string json = File.ReadAllText(CommonHelper.MapPath(filepath + string.Format("{0}.json", "setting")));
          //  var pro = JsonConvert.DeserializeObject<settinglist>(Setting);

            var pro=JsonConvert.DeserializeObject<List<Setting>>(json);
            var settings = pro.ToList();
            return settings;
        }

       
    
        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="storeId">Store identifier</param>
        /// <param name="settings">Setting instance</param>
        public virtual void SaveSetting<T>(T settings) where T : ISettings, new()
        {
            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            foreach (var prop in typeof(T).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                if (!CommonHelper.GetNeoCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                string key = typeof(T).Name + "." + prop.Name;
                //Duck typing is not supported in C#. That's why we're using dynamic type
                dynamic value = prop.GetValue(settings, null);
                if (value != null)
                    SetSetting(key, value, false);
                else
                    SetSetting(key, "", false);
            }

            //and now clear cache
            ClearCache();
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="storeId">Store identifier</param>
        /// <param name="settings">Setting instance</param>
        public virtual void SaveSetting<T>(T settings,string filename) where T : ISettings, new()
        {
            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            foreach (var prop in typeof(T).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                if (!CommonHelper.GetNeoCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                string key = typeof(T).Name + "." + prop.Name;
                //Duck typing is not supported in C#. That's why we're using dynamic type
                dynamic value = prop.GetValue(settings, null);
                if (value != null)
                    SetSetting(key, value, false, filename);
                else
                    SetSetting(key, "", false, filename);
            }

            //and now clear cache
            ClearCache();
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        public virtual void ClearCache()
        {
            _cacheManager.RemoveByPattern(SETTINGS_PATTERN_KEY);
        }

        #endregion
    }
}