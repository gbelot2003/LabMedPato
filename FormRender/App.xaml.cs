using System.Windows;

namespace FormRender
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            if (!MCART.Resources.RTInfo.RTSupport(typeof(App).Assembly) ?? false)
            {
                MessageBox.Show("Esta aplicación o uno de sus componentes se encuentra desactualizado(s).");
            }
        }
    }
}
