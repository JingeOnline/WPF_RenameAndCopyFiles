using HandyControl.Controls;
using Prism.Commands;
using Prism.Mvvm;

namespace WPF_RenameAndCopyFiles.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private int _StepIndex;
        public int StepIndex
        {
            get { return _StepIndex; }
            set { SetProperty(ref _StepIndex, value); }
        }

        public DelegateCommand<StepBar> PrevCommand { get; set; }
        public DelegateCommand<StepBar> NextCommand { get; set; }

        public MainWindowViewModel()
        {
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
    }
}
