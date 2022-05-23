using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_RenameAndCopyFiles.Models;
using WPF_RenameAndCopyFiles.Services;

namespace WPF_RenameAndCopyFiles.ViewModels
{
    public class RenameViewModel : BindableBase, INavigationAware
    {
        private string _RenameAppending;
        public string RenameAppending
        {
            get { return _RenameAppending; }
            set { SetProperty(ref _RenameAppending, value); }
        }

        private ObservableCollection<RenameFileModel> _FileModels;
        public ObservableCollection<RenameFileModel> FileModels
        {
            get { return _FileModels; }
            set { SetProperty(ref _FileModels, value); }
        }

        public RenameViewModel()
        {
            RenameAppending = ConfigurationManager.AppSettings["RenameAppending"];
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            //throw new NotImplementedException();
            FileModels = new ObservableCollection<RenameFileModel>();
            foreach (DirectoryInfo directoryInfo in StaticParaService.StaticTargetFolders)
            {
                foreach (FileInfo fileInfo in StaticParaService.StaticSourceFiles)
                {
                    FileModels.Add(new RenameFileModel(fileInfo.Name,directoryInfo.FullName,RenameAppending));
                }
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            //throw new NotImplementedException();
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //throw new NotImplementedException();
        }
    }
}
