using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
