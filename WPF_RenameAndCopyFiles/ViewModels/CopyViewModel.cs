using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_RenameAndCopyFiles.Services;

namespace WPF_RenameAndCopyFiles.ViewModels
{
    public class CopyViewModel:BindableBase
    {
        private string _ArchiveFolderPath;
        public string ArchiveFolderPath
        {
            get { return _ArchiveFolderPath; }
            set { SetProperty(ref _ArchiveFolderPath, value); }
        }

        public DelegateCommand SelectArchiveFolderCommand { get; set; }

        public CopyViewModel()
        {
            SetArchiveFolderPath();
            SelectArchiveFolderCommand = new DelegateCommand(SelectArchiveFolder);
        }

        private void SelectArchiveFolder()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {

                dialog.IsFolderPicker = true; //Select Folder Only
                dialog.Multiselect = false;

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    ArchiveFolderPath=dialog.FileName;
                }
            }
        }

        private void SetArchiveFolderPath()
        {
            string DateAndTime = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            string FolerName = "Archive " + DateAndTime;
            ArchiveFolderPath = Path.Combine(StaticParaService.SourceFolderPath,FolerName);
        }
    }
}
