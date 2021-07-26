using IBGChatDesctop.Service;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace IBGChatDesctop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnNavigated(NavigationEventArgs e)
        {
            base.OnNavigated(e);
            Page page = e.Content as Page;
            if (page != null)
            {
               NavigationHandler.NavigationService = page.NavigationService;
            }
        }
    }
}
