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
using Microsoft.WindowsAPICodePack.Dialogs;

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

        private DirectoryInfo _SelectedFolder;
        public DirectoryInfo SelectedFolder
        {
            get { return _SelectedFolder; }
            set { SetProperty(ref _SelectedFolder, value); }
        }


        public DelegateCommand AddFolderCommand { get; set; }
        public DelegateCommand RemoveFolderCommand { get; set; }

        public SetTargetViewModel()
        {
            TargetFolders = new ObservableCollection<DirectoryInfo>();
            AddFolderCommand = new DelegateCommand(AddFolder);
            RemoveFolderCommand = new DelegateCommand(RemoveFolder);
            getTargetFolderPathsFromConfig();
        }

        private void RemoveFolder()
        {
            TargetFolders.Remove(SelectedFolder);
        }

        private void AddFolder()
        {
            //throw new NotImplementedException();
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {

                dialog.IsFolderPicker = true; //Select Folder Only
                dialog.Multiselect = false;

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    TargetFolders.Add( new DirectoryInfo(dialog.FileName) );
                }
            }
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
            GlobalStaticService.GlobalTargetFolders = TargetFolders.ToList();
            
            //Save to config
            ConfigService.ClearKeysBySearch("TargetFolderPath");
            for(int i=0;i< TargetFolders.Count; i++)
            {
                ConfigService.CreatKeyValue("TargetFolderPath_"+i+1,TargetFolders[i].FullName);
            }
        }
    }
}
