using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using System.Configuration;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Collections.ObjectModel;
using Prism.Regions;
using WPF_RenameAndCopyFiles.Services;
using System.Windows;

namespace WPF_RenameAndCopyFiles.ViewModels
{
    public class SetSourceViewModel : BindableBase, INavigationAware
    {
        private ObservableCollection<FileInfo> _Files;
        public ObservableCollection<FileInfo> Files
        {
            get { return _Files; }
            set { SetProperty(ref _Files, value); }
        }

        private string _SourceFolderPath;
        public string SourceFolderPath
        {
            get { return _SourceFolderPath; }
            set { SetProperty(ref _SourceFolderPath, value); getFiles(); }
        }

        private string _SourceFolderPathError;
        public string SourceFolderPathError
        {
            get { return _SourceFolderPathError; }
            set { SetProperty(ref _SourceFolderPathError, value); RaisePropertyChanged(nameof(ErrorVisibility)); }
        }

        public Visibility ErrorVisibility
        {
            get
            {
                if (string.IsNullOrEmpty(SourceFolderPathError))
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }

        public DelegateCommand SelectFolderCommand { get; set; }
        public DelegateCommand<string> SourceFolderPathEnterCommand { get; set; }


        public SetSourceViewModel()
        {
            SourceFolderPath=ConfigurationManager.AppSettings["SourceFolderPath"];
            SelectFolderCommand = new DelegateCommand(selectFolder);
            SourceFolderPathEnterCommand = new DelegateCommand<string>(SourceFolderPathEnter);
        }

        private void SourceFolderPathEnter(string path)
        {
            SourceFolderPath = path;
        }

        private void selectFolder()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                
                dialog.IsFolderPicker = true; //Select Folder Only
                dialog.Multiselect = false;

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    SourceFolderPath = dialog.FileName;
                }
            }
        }

        private void getFiles()
        {
            Files = new ObservableCollection<FileInfo>();
            DirectoryInfo sourceFolder = new DirectoryInfo(SourceFolderPath);
            if (sourceFolder.Exists)
            {
                SourceFolderPathError = "";
                foreach (FileInfo file in sourceFolder.GetFiles(".", SearchOption.AllDirectories))
                {
                    Files.Add(file);
                }
            }
            else
            {
                SourceFolderPathError = "⬤ The source folder path is not exist.";
            }


        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

            GlobalStaticService.GlobalSourceFiles = Files.ToList();
            GlobalStaticService.GlobalSourceFolderPath = SourceFolderPath;
            //Save to config
            ConfigService.SaveKeyValue("SourceFolderPath",SourceFolderPath);

        }
    }
}
