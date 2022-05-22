using WPF_RenameAndCopyFiles.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace WPF_RenameAndCopyFiles
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
