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
            set { SetProperty(ref _SourceFolderPath, value); getFiles(); StaticParaService.SourceFolderPath = SourceFolderPath; }
        }

        public DelegateCommand SelectFolderCommand { get; set; }

        public SetSourceViewModel()
        {
            SourceFolderPath=ConfigurationManager.AppSettings["SourceFolderPath"];
            SelectFolderCommand = new DelegateCommand(selectFolder);

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
            foreach (FileInfo file in sourceFolder.GetFiles(".",SearchOption.AllDirectories))
            {
                Files.Add(file);
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            //throw new NotImplementedException();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //throw new NotImplementedException();
            StaticParaService.StaticSourceFiles = Files.ToList();
        }
    }
}
