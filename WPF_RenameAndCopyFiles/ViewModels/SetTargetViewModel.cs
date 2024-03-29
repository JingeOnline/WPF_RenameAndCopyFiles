﻿using System;
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

        private int _TargetFolderNotExistCount;
        public int TargetFolderNotExistCount
        {
            get { return _TargetFolderNotExistCount; }
            set
            {
                SetProperty(ref _TargetFolderNotExistCount, value);
                RaisePropertyChanged(nameof(TargetFolderNotExistCountVisibility));
            }
        }

        public Visibility TargetFolderNotExistCountVisibility
        {
            get
            {
                return TargetFolderNotExistCount > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
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
            set
            {
                if (value == "Template-None")
                {
                    SetProperty(ref _SelectedTemplate, null);
                }
                else
                {
                    SetProperty(ref _SelectedTemplate, value);
                }
                getTargetFolderPathsFromConfig();
            }
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
        public DelegateCommand<string> OpenPathInFileExploreCommand { get; set; }
        private IEventAggregator _eventAggregator;


        public SetTargetViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            TargetFolders = new ObservableCollection<DirectoryInfo>();
            AddFolderCommand = new DelegateCommand(AddFolder);
            RemoveFolderCommand = new DelegateCommand<object>(RemoveFolder);
            AddUserInputPathCommand = new DelegateCommand(addUserInputPath);
            OpenPathInFileExploreCommand = new DelegateCommand<string>(openPathInFileExplore);
            //getTargetFolderPathsFromConfig();
            loadTemplate();
        }

        private void loadTemplate()
        {
            List<string> templates = ConfigService.GetKeyMiddleNamesBySearchKeys("TargetFolderPath");
            //templates.Insert(0, "None");
            templates.Add("Template-None");
            //templates.Add("");
            TemplateNames = new ObservableCollection<string>(templates);
        }

        private async void addUserInputPath()
        {
            if (string.IsNullOrEmpty(UserInputPath)) return;
            try
            {
                _eventAggregator.GetEvent<LoadingOverlayEvent>().Publish(true);
                DirectoryInfo di = null;
                await Task.Run(() =>
                {
                    di = new DirectoryInfo(UserInputPath);
                    //pre-run the file exist check, otherwise it will check at UI thread at the next step.
                    bool isExist = di.Exists;
                });
                if (di != null)
                {
                    TargetFolders.Add(di);
                    UserInputPath = String.Empty;
                    TargetFolderNotExistCount = TargetFolders.Where(x => x.Exists == false).Count();
                }

                _eventAggregator.GetEvent<LoadingOverlayEvent>().Publish(false);
            }
            catch
            {
                _eventAggregator.GetEvent<LoadingOverlayEvent>().Publish(false);
                HandyControl.Controls.MessageBox.Show($"\"{UserInputPath}\"\nCannot be parsed to a directory folder.", "Fail to parse path", MessageBoxButton.OK, MessageBoxImage.Error);
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
            TargetFolderNotExistCount = TargetFolders.Where(x => x.Exists == false).Count();
        }

        private void AddFolder()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {

                dialog.IsFolderPicker = true; //Select Folder Only
                dialog.Multiselect = true;

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    foreach (string filePath in dialog.FileNames)
                    {
                        TargetFolders.Add(new DirectoryInfo(filePath));
                    }
                }
            }
            TargetFolderNotExistCount = TargetFolders.Where(x => x.Exists == false).Count();
        }

        private async void getTargetFolderPathsFromConfig()
        {
            if (String.IsNullOrEmpty(SelectedTemplate))
            {
                TargetFolders.Clear();
                return;
            }
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
                    HandyControl.Controls.MessageBox.Show($"{path}\nCannot be parsed to a directory folder.", "Invalid Path", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            TargetFolderNotExistCount = TargetFolders.Where(x => x.Exists == false).Count();
            //Turn off the loading animation
            _eventAggregator.GetEvent<LoadingOverlayEvent>().Publish(false);
        }

        private void openPathInFileExplore(string path)
        {
            try
            {
                Process.Start(path);
            }
            catch(Exception ex)
            {
                HandyControl.Controls.MessageBox.Show(ex.ToString(),"Cannot Open The Folder Path");
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        //Return true for load an existing instance, false for create a new instance.
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
