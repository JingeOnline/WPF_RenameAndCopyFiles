using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace WPF_RenameAndCopyFiles.Services
{
    public class ConfigService
    {
        public static List<string> GetValueBySearchKeys(string searchTerm)
        {
            var keys = ConfigurationManager.AppSettings.Keys;

            return keys.Cast<object>()
               .Where(key => key.ToString().ToLower()
               .Contains(searchTerm.ToLower()))
               .Select(key => ConfigurationManager.AppSettings.Get(key.ToString())).ToList();
        }

        /// <summary>
        /// If Key=TargetFolderPath-Test-1, Return "Test"
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public static List<string> GetKeyMiddleNamesBySearchKeys(string searchTerm)
        {
            var keys = ConfigurationManager.AppSettings.Keys;

            List<string> keyNames = keys.Cast<string>()
               .Where(key => key.ToString().ToLower()
               .Contains(searchTerm.ToLower())).ToList();

            List<string> middleNames = new List<string>();
            string matchPattern = @"-(\w)*-";
            foreach (string keyName in keyNames)
            {
                Match match = Regex.Match(keyName, matchPattern);
                if (match.Success)
                {
                    string middleName = match.ToString().Trim('-');
                    if (!middleNames.Contains(middleName))
                    {
                        middleNames.Add(middleName);
                    }
                }
            }
            return middleNames;
        }

        public static void ClearKeysBySearch(string searchTerm)
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var keys = ConfigurationManager.AppSettings.Keys;

            foreach (var key in keys)
            {
                if (key.ToString().ToLower().Contains(searchTerm.ToLower()))
                {
                    cfa.AppSettings.Settings.Remove(key.ToString());
                }
            }

            cfa.Save();
        }

        public static void CreatKeyValue(string key, string value)
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings.Add(key, value);
            cfa.Save();
        }

        public static void SaveKeyValue(string key, string value)
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings[key].Value = value;
            cfa.Save();
        }
    }
}
