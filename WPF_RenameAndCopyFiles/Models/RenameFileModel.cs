using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_RenameAndCopyFiles.Models
{
    public class RenameFileModel:BindableBase
    {
        private string _OrigionalName;
        public string OrigionalName
        {
            get { return _OrigionalName; }
            set { SetProperty(ref _OrigionalName, value); }
        }

        private string _NewName;
        public string NewName
        {
            get { return _NewName; }
            set { SetProperty(ref _NewName, value); }
        }

        private string _OrigionalFilePath;
        public string OrigionalFilePath
        {
            get { return _OrigionalFilePath; }
            set { SetProperty(ref _OrigionalFilePath, value); }
        }

        private string _NewFilePath;
        public string NewFilePath
        {
            get { return _NewFilePath; }
            set { SetProperty(ref _NewFilePath, value); }
        }

        private bool _IsDone;
        public bool IsDone
        {
            get { return _IsDone;}
            set { SetProperty(ref _IsDone, value); }
        }

        private bool _IsExist;
        public bool IsExist
        {
            get { return _IsExist; }
            set { SetProperty(ref _IsExist,value); }
        }

        private string _Message;
        public string Message
        {
            get { return _Message; }
            set { SetProperty(ref _Message, value); }
        }

        public RenameFileModel(string fileName, string folderPath, string renameAppending )
        {
            OrigionalName= fileName;
            OrigionalFilePath = Path.Combine(folderPath, fileName);
            NewName = fileName + renameAppending;
            NewFilePath = Path.Combine(folderPath,NewName);
            IsExist=File.Exists(OrigionalFilePath);
        }


    }
}
