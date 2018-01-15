using System.Windows;
using static TheXDS.MCART.Resources.RTInfo;
using TheXDS.MCART;
using TheXDS.MCART.Attributes;
namespace FormRender
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            if (!RTSupport(typeof(App).Assembly) ?? false)
            {
                MessageBox.Show(
                    $"Esta aplicación o uno de sus componentes se encuentra" +
                    $" desactualizado(s). Se requiere MCART {typeof(App).Assembly.GetAttr<MinMCARTVersionAttribute>().Value}");
            }
        }
    }
}
