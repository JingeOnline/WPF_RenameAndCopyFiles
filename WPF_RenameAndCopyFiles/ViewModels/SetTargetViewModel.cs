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
using HandyControl.Controls;
using System.Windows;
using System.Collections;
using Prism.Events;
using WPF_RenameAndCopyFiles.Events;

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

        private ObservableCollection<string> _TemplateNames;
        public ObservableCollection<string> TemplateNames
        {
            get { return _TemplateNames; }
            set { SetProperty(ref _TemplateNames, value); }
        }

        private string _SelectedTemplate;
        public string SelectedTemplate
        {
            get { return _SelectedTemplate; }
            set { SetProperty(ref _SelectedTemplate, value); getTargetFolderPathsFromConfig(); }
        }

        //private DirectoryInfo _SelectedFolder;
        //public DirectoryInfo SelectedFolder
        //{
        //    get { return _SelectedFolder; }
        //    set { SetProperty(ref _SelectedFolder, value); }
        //}

        //private List<DirectoryInfo> _SelectedFolders;
        //public List<DirectoryInfo> SelectedFolders
        //{
        //    get { return _SelectedFolders; }
        //    set { SetProperty(ref _SelectedFolders, value); }
        //}

        private string _UserInputPath;
        public string UserInputPath
        {
            get { return _UserInputPath; }
            set { SetProperty(ref _UserInputPath, value); }
        }

        public DelegateCommand AddUserInputPathCommand { get; set; }
        //public DelegateCommand UserInputPathEnterCommand { get; set; }
        public DelegateCommand AddFolderCommand { get; set; }
        public DelegateCommand<Object> RemoveFolderCommand { get; set; }
        private IEventAggregator _eventAggregator;


        public SetTargetViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            TargetFolders = new ObservableCollection<DirectoryInfo>();
            AddFolderCommand = new DelegateCommand(AddFolder);
            RemoveFolderCommand = new DelegateCommand<object>(RemoveFolder);
            AddUserInputPathCommand = new DelegateCommand(addUserInputPath);
            //getTargetFolderPathsFromConfig();
            getTemplate();
        }

        private void getTemplate()
        {
            List<string> templates = ConfigService.GetKeyMiddleNamesBySearchKeys("TargetFolderPath");
            //templates.Insert(0, "None");
            templates.Add("None");
            TemplateNames = new ObservableCollection<string>(templates);
        }

        private void addUserInputPath()
        {
            if (string.IsNullOrEmpty(UserInputPath)) return;
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(UserInputPath);
                TargetFolders.Add(directoryInfo);
                UserInputPath = String.Empty;
            }
            catch
            {
                HandyControl.Controls.MessageBox.Show($"{UserInputPath} \nCannot be parsed to a directory folder.", "Fail to parse path", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Get the user selected rows in the Gatagrid, then delete them.
        /// </summary>
        /// <param name="selectedItems"></param>
        private void RemoveFolder(object selectedItems)
        {
            IList objectList = selectedItems as IList;
            List<DirectoryInfo> selectedDirectoryInfos = objectList.Cast<DirectoryInfo>().ToList();

            foreach (DirectoryInfo directoryInfo in selectedDirectoryInfos)
            {
                TargetFolders.Remove(directoryInfo);
            }
        }

        private void AddFolder()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {

                dialog.IsFolderPicker = true; //Select Folder Only
                dialog.Multiselect = true;

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    //TargetFolders.Add(new DirectoryInfo(dialog.FileName));
                    foreach (string filePath in dialog.FileNames)
                    {
                        TargetFolders.Add(new DirectoryInfo(filePath));
                    }
                }
            }
        }

        private async void getTargetFolderPathsFromConfig()
        {
            //Turn on the loading animation
            _eventAggregator.GetEvent<LoadingOverlayEvent>().Publish(true);
            //List<string> paths = ConfigService.GetValueBySearchKeys("TargetFolderPath" + "-" + SelectedTemplate);
            List<string> paths = ConfigService.GetValueBySearchKeys(SelectedTemplate);
            TargetFolders.Clear();
            foreach (string path in paths)
            {
                try
                {
                    DirectoryInfo di = null;
                    await Task.Run(() =>
                    {
                        di = new DirectoryInfo(path);
                        //pre-run the file exist check, otherwise it will check at UI thread at the next step.
                        bool isExist = di.Exists;
                    });
                    if (di != null)
                    {
                        TargetFolders.Add(di);
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                    HandyControl.Controls.MessageBox.Show($"{path}\nCannot be parsed to a directory folder.", "Fail to parse path from config", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            //Turn off the loading animation
            _eventAggregator.GetEvent<LoadingOverlayEvent>().Publish(false);
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
            GlobalStaticService.GlobalTargetFolders = TargetFolders.ToList();

            //Save to config
            //ConfigService.ClearKeysBySearch("TargetFolderPath");
            //for (int i = 0; i < TargetFolders.Count; i++)
            //{
            //    int index = i + 1;
            //    ConfigService.CreatKeyValue("TargetFolderPath_" + index, TargetFolders[i].FullName);
            //}
        }
    }
}
