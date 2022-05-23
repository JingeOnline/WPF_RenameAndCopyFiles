using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.IO;
using System.Configuration;
using WPF_RenameAndCopyFiles.Services;
using Prism.Regions;
using System.Diagnostics;

namespace WPF_RenameAndCopyFiles.ViewModels
{
    public class SetTargetViewModel : BindableBase, INavigationAware
    {
        private ObservableCollection<DirectoryInfo> _TargetFolders;
        public ObservableCollection<DirectoryInfo> TargetFolders
        {
            get { return _TargetFolders; }
            set { SetProperty(ref _TargetFolders, value); }
        }

        public SetTargetViewModel()
        {
            TargetFolders = new ObservableCollection<DirectoryInfo>();
            getTargetFolderPathsFromConfig();
        }

        private void getTargetFolderPathsFromConfig()
        {
            IEnumerable<string> paths = ConfigService.GetValueBySearchKeys("TargetFolderPath");
            foreach (string path in paths)
            {
                TargetFolders.Add(new DirectoryInfo(path));
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            //throw new NotImplementedException();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            //throw new NotImplementedException();
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //throw new NotImplementedException();
            StaticParaService.StaticTargetFolders = TargetFolders.ToList();
        }
    }
}
