using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Commands;
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
using System.Windows;
using WPF_RenameAndCopyFiles.Models;
using WPF_RenameAndCopyFiles.Services;

namespace WPF_RenameAndCopyFiles.ViewModels
{
    public class RenameViewModel : BindableBase, INavigationAware
    {
        //private string _RenameAppending;
        //public string RenameAppending
        //{
        //    get { return _RenameAppending; }
        //    set { SetProperty(ref _RenameAppending, value); }
        //}
        private string _ArchiveFolderPath;
        public string ArchiveFolderPath
        {
            get { return _ArchiveFolderPath; }
            set { SetProperty(ref _ArchiveFolderPath, value); checkIfArchiveFolderExist(); }
        }

        private string _ArchiveFolderPathError;
        public string ArchiveFolderPathError
        {
            get { return _ArchiveFolderPathError; }
            set { SetProperty(ref _ArchiveFolderPathError, value); RaisePropertyChanged(nameof(ErrorVisibility)); }
        }

        private ObservableCollection<RenameFileModel> _FileModels;
        public ObservableCollection<RenameFileModel> FileModels
        {
            get { return _FileModels; }
            set { SetProperty(ref _FileModels, value); }
        }

        private Visibility _IsDoneColumnVisibility;
        public Visibility IsDoneColumnVisibility
        {
            get { return _IsDoneColumnVisibility; }
            set { SetProperty(ref _IsDoneColumnVisibility, value); }
        }

        public Visibility ErrorVisibility
        {
            get
            {
                if (string.IsNullOrEmpty(ArchiveFolderPathError))
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }

        public DelegateCommand RenameCommand { get; set; }
        public DelegateCommand SelectFolderCommand { get; set; }
        public DelegateCommand<string> ArchiveFolderPathEnterCommand { get; set; }

        public RenameViewModel()
        {
            RenameCommand = new DelegateCommand(renameAndArchive);
            SelectFolderCommand = new DelegateCommand(selectFolder);
            ArchiveFolderPathEnterCommand = new DelegateCommand<string>(ArchiveFolderPathEnter);
            IsDoneColumnVisibility = Visibility.Collapsed;
            ArchiveFolderPath= ConfigurationManager.AppSettings["SourceArchiveFolderPath"];

        }

        private void ArchiveFolderPathEnter(string path)
        {
            ArchiveFolderPath = path;
        }

        private void selectFolder()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {

                dialog.IsFolderPicker = true; //Select Folder Only
                dialog.Multiselect = false;

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    ArchiveFolderPath = dialog.FileName;
                }
            }
        }

        private void renameAndArchive()
        {
            //Rename
            foreach(RenameFileModel file in FileModels.Where(x=>x.IsExist))
            {
                try
                {
                    File.Move(file.OrigionalFilePath, file.NewFilePath);
                    file.IsDone = true;
                    IsDoneColumnVisibility = Visibility.Visible;
                    file.Message = "Successful";
                }
                catch(Exception ex)
                {
                    file.Message = ex.Message+"\r\n"+ex.InnerException.Message;
                }
            }
            //Archive
            //Create Folder
            string folderName = "Backup " + DateTime.Now.ToString("yyyy-MM-dd # HH-mm-ss");
            string archiveFolder = Path.Combine(ArchiveFolderPath, folderName);
            Directory.CreateDirectory(archiveFolder);
            //Move
            foreach(FileInfo file in GlobalStaticService.GlobalSourceFiles)
            {
                File.Move(file.FullName, Path.Combine(archiveFolder, file.Name));
            }
        }


        private void checkIfArchiveFolderExist()
        {
            DirectoryInfo archiveFolder = new DirectoryInfo(ArchiveFolderPath);
            if (!archiveFolder.Exists)
            {
                ArchiveFolderPathError = "⬤ The archive folder path is not exist.";
            }
            else
            {
                ArchiveFolderPathError = string.Empty;
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            FileModels = new ObservableCollection<RenameFileModel>();
            string renameAppending = ConfigurationManager.AppSettings["RenameAppending"];

            foreach (DirectoryInfo directoryInfo in GlobalStaticService.GlobalTargetFolders)
            {
                foreach (FileInfo fileInfo in GlobalStaticService.GlobalSourceFiles)
                {
                    FileModels.Add(new RenameFileModel(fileInfo,directoryInfo.FullName,renameAppending));
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
