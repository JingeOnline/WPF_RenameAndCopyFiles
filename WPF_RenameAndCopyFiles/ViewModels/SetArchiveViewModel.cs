﻿using HandyControl.Controls;
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
using WPF_RenameAndCopyFiles.Services;

namespace WPF_RenameAndCopyFiles.ViewModels
{
    public class SetArchiveViewModel : BindableBase, INavigationAware
    {
        private string _SourceArchiveFolderPath;
        public string SourceArchiveFolderPath
        {
            get { return _SourceArchiveFolderPath; }
            set { SetProperty(ref _SourceArchiveFolderPath, value); checkSourcePathExist(); }
        }

        private string _TargetArchiveFolderPath;
        public string TargetArchiveFolderPath
        {
            get { return _TargetArchiveFolderPath; }
            set 
            { 
                SetProperty(ref _TargetArchiveFolderPath, value);
                //验证用户手动输入的地址是否合法
                if (_TargetArchiveFolderPath.IndexOfAny("?/:*\"<>|".ToCharArray())!=-1)
                {
                    TargetArchiveFolderPathError = "A valid path in windows system cannot contains ?/:*\"<>|";
                    RaisePropertyChanged(nameof(TargetPathErrorVisibility));
                }
                else
                {
                    showTargetArchiveFolders();
                    TargetArchiveFolderPathError = String.Empty;
                    RaisePropertyChanged(nameof(TargetPathErrorVisibility));
                }
            }
        }

        private string _SourceArchiveFolderPathError;
        public string SourceArchiveFolderPathError
        {
            get { return _SourceArchiveFolderPathError; }
            set { SetProperty(ref _SourceArchiveFolderPathError, value); RaisePropertyChanged(nameof(SourcePathErrorVisibility)); }
        }

        private string _TargetArchiveFolderPathError;
        public string TargetArchiveFolderPathError
        {
            get { return _TargetArchiveFolderPathError; }
            set { SetProperty(ref _TargetArchiveFolderPathError, value); }
        }

        private bool _IsSourceNeedArchive;
        public bool IsSourceNeedArchive
        {
            get { return _IsSourceNeedArchive; }
            set { SetProperty(ref _IsSourceNeedArchive, value); }
        }

        public Dictionary<string, string> folerToArchiveFolder { get; set; } = new Dictionary<string, string>();

        public Visibility SourcePathErrorVisibility
        {
            get
            {
                if (string.IsNullOrEmpty(SourceArchiveFolderPathError))
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }

        public Visibility TargetPathErrorVisibility
        {
            get
            {
                if (string.IsNullOrEmpty(TargetArchiveFolderPathError))
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }

        private ObservableCollection<DirectoryInfo> _TargetArchiveFolders;
        public ObservableCollection<DirectoryInfo> TargetArchiveFolders
        {
            get { return _TargetArchiveFolders; }
            set { SetProperty(ref _TargetArchiveFolders, value); }
        }


        public DelegateCommand<string> SourceArchiveFolderPathEnterCommand { get; set; }
        public DelegateCommand<string> TargetArchiveFolderPathEnterCommand { get; set; }
        public DelegateCommand SelectSourceFolderCommand { get; set; }
        public DelegateCommand SelectTargetFolderCommand { get; set; }
        public DelegateCommand CreateFolderIfNotExistCommand { get; set; }


        public SetArchiveViewModel()
        {
            SourceArchiveFolderPathEnterCommand = new DelegateCommand<string>(SourceArchiveFolderPathEnter);
            TargetArchiveFolderPathEnterCommand = new DelegateCommand<string>(TargetArchiveFolderPathEnter);
            SelectSourceFolderCommand = new DelegateCommand(SelectSourceArchiveFolder);
            SelectTargetFolderCommand = new DelegateCommand(SelectTargetArchiveFolder);

            CreateFolderIfNotExistCommand = new DelegateCommand(creatFolderIfNotExist, canCreateFolderIfNotExist).ObservesProperty(()=>TargetArchiveFolderPathError);

            //SourceArchiveFolderPath = ConfigurationManager.AppSettings["SourceArchiveFolderPath"] + "\\Backup " + DateTime.Now.ToString("yyyy-MM-dd #HH#mm#ss");
            //TargetArchiveFolderPath = ConfigurationManager.AppSettings["TargetArchiveFolderPath"]+"\\Backup "+DateTime.Now.ToString("yyyy-MM-dd #HH#mm#ss");
        }

        private bool canCreateFolderIfNotExist()
        {
            //return true;
            return TargetArchiveFolderPathError == String.Empty;
        }

        private async void creatFolderIfNotExist()
        {
            DirectoryInfo sourceFolder = new DirectoryInfo(SourceArchiveFolderPath);
            if (!sourceFolder.Exists && IsSourceNeedArchive)
            {
                Directory.CreateDirectory(SourceArchiveFolderPath);
            }

            foreach (DirectoryInfo folder in TargetArchiveFolders.Where(x => x.Exists == false))
            {
                Directory.CreateDirectory(folder.FullName);
            }
            await Task.Delay(500);
            showTargetArchiveFolders();
            checkSourcePathExist();
        }

        private void showTargetArchiveFolders()
        {
            string targetArchivePath=String.Empty;
            try
            {
                TargetArchiveFolders = new ObservableCollection<DirectoryInfo>();
                foreach (DirectoryInfo folder in GlobalStaticService.GlobalTargetFolders)
                {
                    targetArchivePath = Path.Combine(folder.FullName, TargetArchiveFolderPath);
                    TargetArchiveFolders.Add(new DirectoryInfo(targetArchivePath));
                    folerToArchiveFolder[folder.FullName] = targetArchivePath;
                }
            }
            catch(ArgumentException ex)
            {
                string path = targetArchivePath == String.Empty ? TargetArchiveFolderPath : targetArchivePath;
                HandyControl.Controls.MessageBox.Show(ex.Message+"\r\n"+path,"Invalid Path",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            catch(Exception ex)
            {
                HandyControl.Controls.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void checkSourcePathExist()
        {
            DirectoryInfo sourceFolder = new DirectoryInfo(SourceArchiveFolderPath);
            if (!sourceFolder.Exists)
            {
                SourceArchiveFolderPathError = "⬤ The source archive folder path is not exist.";
            }
            else
            {
                SourceArchiveFolderPathError = string.Empty;
            }
        }

        private void SelectTargetArchiveFolder()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {

                dialog.IsFolderPicker = true; //Select Folder Only
                dialog.Multiselect = false;

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    TargetArchiveFolderPath = dialog.FileName;
                }
            }
        }

        private void SelectSourceArchiveFolder()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {

                dialog.IsFolderPicker = true; //Select Folder Only
                dialog.Multiselect = false;

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    SourceArchiveFolderPath = dialog.FileName + "\\Backup " + DateTime.Now.ToString("yyyy-MM-dd #HH#mm#ss");
                }
            }
        }

        private void TargetArchiveFolderPathEnter(string path)
        {
            TargetArchiveFolderPath = path;
        }

        private void SourceArchiveFolderPathEnter(string path)
        {
            SourceArchiveFolderPath = path;
        }

        //Navigate into this page
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (SourceArchiveFolderPath == null)
            {
                SourceArchiveFolderPath = ConfigurationManager.AppSettings["SourceArchiveFolderPath"] + "\\Backup " + DateTime.Now.ToString("yyyy-MM-dd #HH#mm#ss");
            }
            if (TargetArchiveFolderPath == null)
            {
                TargetArchiveFolderPath = ConfigurationManager.AppSettings["TargetArchiveFolderPath"] + "\\Backup " + DateTime.Now.ToString("yyyy-MM-dd #HH#mm#ss");
            }
        }

        //Return true for load an existing instance, false for create a new instance.
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            //return false;
            return true;
        }

        //Navigate out from this page
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            GlobalStaticService.GlobalSourceArchiveFolderPath = SourceArchiveFolderPath;
            GlobalStaticService.GlobalTargetFolderAndArchiveFolderPaths = folerToArchiveFolder;
            GlobalStaticService.IsSourceNeedArchive = IsSourceNeedArchive;

            ConfigService.SaveKeyValue("SourceArchiveFolderPath", new DirectoryInfo(SourceArchiveFolderPath).Parent.FullName);
            //GlobalStaticService.GlobalTargetArchiveFolderPaths = TargetArchiveFolders.ToList();
        }
    }
}
