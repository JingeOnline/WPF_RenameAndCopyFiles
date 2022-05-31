using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPF_RenameAndCopyFiles.Services;




namespace WPF_RenameAndCopyFiles.ViewModels
{
    public class ExecuteViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand ExecuteCommand { get; set; }
        public DelegateCommand ExitCommand { get; set; }

        private bool _CanExecute;
        public bool CanExecute
        {
            get { return _CanExecute; }
            set { SetProperty(ref _CanExecute, value); }
        }

        private bool _IsDone;
        public bool IsDone
        {
            get { return _IsDone; }
            set { SetProperty(ref _IsDone, value); }
        }


        private bool _IsFinish1;
        public bool IsFinish1
        {
            get { return _IsFinish1; }
            set { SetProperty(ref _IsFinish1, value); }
        }

        private bool _IsFinish2;
        public bool IsFinish2
        {
            get { return _IsFinish2; }
            set { SetProperty(ref _IsFinish2, value); }
        }

        private bool _IsFinish3;
        public bool IsFinish3
        {
            get { return _IsFinish3; }
            set { SetProperty(ref _IsFinish3, value); }
        }

        private double _ProgressBarValue1;
        public double ProgressBarValue1
        {
            get { return _ProgressBarValue1; }
            set { SetProperty(ref _ProgressBarValue1, value); }
        }

        private double _ProgressBarValue2;
        public double ProgressBarValue2
        {
            get { return _ProgressBarValue2; }
            set { SetProperty(ref _ProgressBarValue2, value); }
        }
        private double _ProgressBarValue3;
        public double ProgressBarValue3
        {
            get { return _ProgressBarValue3; }
            set { SetProperty(ref _ProgressBarValue3, value); }
        }
        private bool _IsSourceNeedArchive;
        public bool IsSourceNeedArchive
        {
            get { return _IsSourceNeedArchive; }
            set { SetProperty(ref _IsSourceNeedArchive, value); }
        }
        public ObservableCollection<Exception> Exception1 { get; set; } = new ObservableCollection<Exception>();
        public ObservableCollection<Exception> Exception2 { get; set; } = new ObservableCollection<Exception>();
        public ObservableCollection<Exception> Exception3 { get; set; } = new ObservableCollection<Exception>();

        public ExecuteViewModel()
        {
            //ExecuteCommand = new DelegateCommand(execute).ObservesCanExecute(() => CanExecute);
            ExecuteCommand = new DelegateCommand(async () => await execute()).ObservesCanExecute(() => CanExecute);
            //ExecuteCommand = new DelegateCommand(async () => await execute());
            ExitCommand = new DelegateCommand(exit);
            CanExecute = true;
            IsDone = false;
            IsSourceNeedArchive = GlobalStaticService.IsSourceNeedArchive;
        }

        private void exit()
        {
            Application.Current.Shutdown();
        }

        //private bool canExecute()
        //{
        //    //Debug.WriteLine(CanExecute);
        //    return CanExecute;
        //}

        private async Task execute()
        {

            CanExecute = false;
            await moveTargetFileToArchive();
            await copyFileToTarget();
            if (IsSourceNeedArchive)
            {
                await moveSourceFileToArchive();
            }
            IsDone = true;
            //CanExecute = true;

        }

        private async Task moveTargetFileToArchive()
        {
            int index = 0;
            foreach (DirectoryInfo folder in GlobalStaticService.GlobalTargetFolders)
            {
                string archiveFolderPath = GlobalStaticService.GlobalTargetFolderAndArchiveFolderPaths[folder.FullName];
                Dictionary<string, string> sourceFileToTargetFile = getTargetFilePaths(folder.FullName);

                foreach (var pair in sourceFileToTargetFile)
                {
                    string fileName = new FileInfo(pair.Value).Name;
                    string arciveFilePath = Path.Combine(archiveFolderPath, fileName);
                    try
                    {
                        await Task.Run(() =>
                        {
                            File.Move(pair.Value, arciveFilePath);
                        });
                    }
                    catch (Exception ex)
                    {
                        Exception1.Add(ex);
                    }

                    index++;
                    ProgressBarValue1 = index * 100 / (GlobalStaticService.GlobalTargetFolders.Count * sourceFileToTargetFile.Count);
                    //Debug.WriteLine("ProgressBarValue1=" + ProgressBarValue1);
                }
            }

            IsFinish1 = true;
        }

        private async Task copyFileToTarget()
        {
            int index = 0;
            foreach (DirectoryInfo folder in GlobalStaticService.GlobalTargetFolders)
            {
                Dictionary<string, string> sourceFileToTargetFile = getTargetFilePaths(folder.FullName);
                foreach (var pair in sourceFileToTargetFile)
                {
                    try
                    {
                        await Task.Run(() =>
                        {
                            File.Copy(pair.Key, pair.Value);
                        });
                    }
                    catch (Exception ex)
                    {
                        Exception2.Add(ex);
                    }
                    index++;
                    ProgressBarValue2 = index * 100 / (GlobalStaticService.GlobalTargetFolders.Count * sourceFileToTargetFile.Count);
                    //Debug.WriteLine("ProgressBarValue2=" + ProgressBarValue2);
                }
            }
            IsFinish2 = true;
        }


        private async Task moveSourceFileToArchive()
        {
            int index = 0;
            foreach (FileInfo file in GlobalStaticService.GlobalSourceFiles)
            {
                string archiveFilePath = Path.Combine(GlobalStaticService.GlobalSourceArchiveFolderPath, file.Name);
                try
                {
                    await Task.Run(() =>
                    {
                        File.Move(file.FullName, archiveFilePath);
                    });
                }
                catch (Exception ex)
                {
                    Exception3.Add(ex);
                }
                index++;
                ProgressBarValue3 = index * 100 / GlobalStaticService.GlobalSourceFiles.Count;
                //Debug.WriteLine("ProgressBarValue3=" + ProgressBarValue3);
            }
            IsFinish3 = true;
        }

        private Dictionary<string, string> getTargetFilePaths(string targetFolderPath)
        {
            Dictionary<string, string> sourceFileToTargetFile = new Dictionary<string, string>();
            foreach (FileInfo file in GlobalStaticService.GlobalSourceFiles)
            {
                string relativePath = file.FullName.Substring(GlobalStaticService.GlobalSourceFolderPath.Length);
                string path = Path.Combine(targetFolderPath, relativePath.TrimStart('\\', '/'));
                sourceFileToTargetFile[file.FullName] = path;
            }
            return sourceFileToTargetFile;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            //throw new NotImplementedException();
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //throw new NotImplementedException();
        }
    }
}
