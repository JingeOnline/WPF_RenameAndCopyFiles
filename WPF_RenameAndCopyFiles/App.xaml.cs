using WPF_RenameAndCopyFiles.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using Prism.Regions;

namespace WPF_RenameAndCopyFiles
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            IRegionManager regionManager = Container.Resolve<IRegionManager>();
            //Set the initial navigation view
            regionManager.RegisterViewWithRegion("ContentRegion", typeof(SetSourceView));
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SetSourceView>();
            containerRegistry.RegisterForNavigation<SetTargetView>();
            containerRegistry.RegisterForNavigation<RenameView>();
            containerRegistry.RegisterForNavigation<ArchiveView>();
        }
    }
}
