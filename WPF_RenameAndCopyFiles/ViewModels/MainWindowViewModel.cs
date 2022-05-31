using HandyControl.Controls;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using WPF_RenameAndCopyFiles.Events;
using WPF_RenameAndCopyFiles.Views;

namespace WPF_RenameAndCopyFiles.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private IRegionManager _regionManager;
        //private IEventAggregator _eventAggregator;

        private int _StepIndex;
        public int StepIndex
        {
            get { return _StepIndex; }
            set { SetProperty(ref _StepIndex, value); Navigation(); Debug.WriteLine(StepIndex); }
        }

        private bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { SetProperty(ref _IsLoading,value); }
        }

        public DelegateCommand<StepBar> PrevCommand { get; set; }
        public DelegateCommand<StepBar> NextCommand { get; set; }



        public MainWindowViewModel(IRegionManager regionManager,IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<LoadingOverlayEvent>().Subscribe(SetLoadingOverLay);
            _regionManager = regionManager;
            PrevCommand = new DelegateCommand<StepBar>(Prev);
            NextCommand = new DelegateCommand<StepBar>(Next);
        }

        private void SetLoadingOverLay(bool isLoading)
        {
            IsLoading = isLoading;
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
                case 0: _regionManager.RequestNavigate("ContentRegion", nameof(SetSourceView)); break;
                case 1: _regionManager.RequestNavigate("ContentRegion", nameof(SetTargetView)); break;
                case 2: _regionManager.RequestNavigate("ContentRegion", nameof(SetArchiveView)); break;
                case 3: _regionManager.RequestNavigate("ContentRegion", nameof(ExecuteView)); break;
                    //case 3: _regionManager.RequestNavigate("ContentRegion", "RenameView"); break;
            }
        }
    }
}
