using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_RenameAndCopyFiles.Services
{
    public class StaticParaService
    {
        public static List<FileInfo> StaticSourceFiles = new List<FileInfo>();
        public static List<DirectoryInfo> StaticTargetFolders = new List<DirectoryInfo>();
    }
}
