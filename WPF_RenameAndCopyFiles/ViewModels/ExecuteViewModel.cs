using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_RenameAndCopyFiles.Services;




namespace WPF_RenameAndCopyFiles.ViewModels
{
    public class ExecuteViewModel : BindableBase
    {
        public DelegateCommand ExecuteCommand { get; set; }

        private bool _CanExecute;
        public bool CanExecute
        {
            get { return _CanExecute; }
            set { SetProperty(ref _CanExecute, true); }
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
        public ObservableCollection<Exception> Exception1 { get; set; } = new ObservableCollection<Exception>();
        public ObservableCollection<Exception> Exception2 { get; set; } = new ObservableCollection<Exception>();
        public ObservableCollection<Exception> Exception3 { get; set; } = new ObservableCollection<Exception>();

        public ExecuteViewModel()
        {
            CanExecute = true;
            ExecuteCommand = new DelegateCommand(execute).ObservesCanExecute(() => CanExecute);
            //ExecuteCommand = new DelegateCommand(async () => await execute()).ObservesCanExecute(() => CanExecute);
        }

        private async void execute()
        {
            await Task.Run(() =>
            {
                CanExecute = false;
                moveTargetFileToArchive();
                copyFileToTarget();
                moveSourceFileToArchive();
                CanExecute = true;
            });
        }

        private void moveTargetFileToArchive()
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
                        File.Move(pair.Value, arciveFilePath);
                    }
                    catch (Exception ex)
                    {
                        App.Current.Dispatcher.Invoke((Action)delegate
                        {
                            Exception1.Add(ex);
                        });
                    }

                }
                index++;
                ProgressBarValue1 = index / sourceFileToTargetFile.Count * 100;
            }

            IsFinish1 = true;
        }

        private void copyFileToTarget()
        {
            int index = 0;
            foreach (DirectoryInfo folder in GlobalStaticService.GlobalTargetFolders)
            {
                Dictionary<string, string> sourceFileToTargetFile = getTargetFilePaths(folder.FullName);
                foreach (var pair in sourceFileToTargetFile)
                {
                    try
                    {
                        File.Copy(pair.Key, pair.Value);
                    }
                    catch (Exception ex)
                    {
                        App.Current.Dispatcher.Invoke((Action)delegate
                        {
                            Exception2.Add(ex);
                        });
                    }
                }
                index++;
                ProgressBarValue1 = index / GlobalStaticService.GlobalTargetFolders.Count * 100;
            }
            IsFinish2 = true;
        }


        private void moveSourceFileToArchive()
        {
            int index = 0;
            foreach (FileInfo file in GlobalStaticService.GlobalSourceFiles)
            {
                string archiveFilePath = Path.Combine(GlobalStaticService.GlobalSourceArchiveFolderPath, file.Name);
                try
                {
                    File.Move(file.FullName, archiveFilePath);
                }
                catch (Exception ex)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        Exception3.Add(ex);
                    });
                }
                index++;
                ProgressBarValue1 = index / GlobalStaticService.GlobalSourceFiles.Count * 100;
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

    }
}
