using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_RenameAndCopyFiles.Models
{
    public class CopyFileModel:BindableBase
    {
        //private string _TargetFolder;
        //public string TargetFolder
        //{
        //    get { return _TargetFolder; }
        //    set { SetProperty(ref _TargetFolder, value); }
        //}

        private string _TargetFolderPath;
        public string TargetFolderPath
        {
            get { return _TargetFolderPath; }
            set { SetProperty(ref _TargetFolderPath, value); }
        }

        private bool _IsDone;
        public bool IsDone
        {
            get { return _IsDone; }
            set { SetProperty(ref _IsDone, value); }
        }

        private string _Message;
        public string Message
        {
            get { return _Message; }
            set { SetProperty(ref _Message, value); }
        }

        public CopyFileModel(string targetFolderPath)
        {
            TargetFolderPath = targetFolderPath;
        }
    }
}
