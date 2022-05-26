using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_RenameAndCopyFiles.Services
{
    public class GlobalStaticService
    {
        public static List<FileInfo> GlobalSourceFiles = new List<FileInfo>();
        public static List<DirectoryInfo> GlobalTargetFolders = new List<DirectoryInfo>();
        public static string GlobalSourceFolderPath;
        public static string GlobalSourceArchiveFolderPath;
        public static Dictionary<string,string> GlobalTargetFolderAndArchiveFolderPaths;
    }
}
