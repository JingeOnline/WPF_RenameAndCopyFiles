using HandyControl.Controls;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace WPF_RenameAndCopyFiles.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private IRegionManager _regionManager;

        private int _StepIndex;
        public int StepIndex
        {
            get { return _StepIndex; }
            set { SetProperty(ref _StepIndex, value); Navigation(); Debug.WriteLine(StepIndex); }
        }

        public DelegateCommand<StepBar> PrevCommand { get; set; }
        public DelegateCommand<StepBar> NextCommand { get; set; }



        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            PrevCommand = new DelegateCommand<StepBar>(Prev);
            NextCommand = new DelegateCommand<StepBar>(Next);
        }

        private void Next(StepBar stepBar)
        {
            stepBar.Next();
        }

        private void Prev(StepBar stepBar)
        {
            stepBar.Prev();
        }

        private void Navigation()
        {
            switch (StepIndex)
            {
                case 0: _regionManager.RequestNavigate("ContentRegion", "SetSourceView"); break;
                case 1: _regionManager.RequestNavigate("ContentRegion", "SetTargetView"); break;
                case 2: _regionManager.RequestNavigate("ContentRegion", "ArchiveView"); break;
                case 3: _regionManager.RequestNavigate("ContentRegion", "ExecuteView"); break;
                    //case 3: _regionManager.RequestNavigate("ContentRegion", "RenameView"); break;
            }
        }
    }
}
