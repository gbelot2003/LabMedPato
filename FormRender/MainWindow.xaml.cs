using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FormRender.Models;
using FormRender.Pages;

namespace FormRender
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Diagnostico diagnostico = new Diagnostico
            {
                Texto = Lorem + Lorem,                
                RutaImagen = new string[]
                {
                    @"C:\Users\xds_x\src\LabMedPato\FormRender\TestImages\Aqua.jpg",
                    @"C:\Users\xds_x\src\LabMedPato\FormRender\TestImages\Bricks2.png",
                    @"C:\Users\xds_x\src\LabMedPato\FormRender\TestImages\Grass.jpg"
                }
            };

            HeaderInfo headerInfo = new HeaderInfo
            {
                Paciente = "Juan Rodríguez",
                Medico = "Dr. José López",
                Direccion = "123 Algún lugar, Tgu",
                Diag = "Ejemplo",
                Estudiado = "Órganos",
                Edad  = 25,
                Sexo = 'M',
                Fecha = DateTime.Today,
                Recibido = DateTime.Now,
                Biopsia = "123-4567-890123"
            };

            frmContent.Navigate(new FormPage(headerInfo,diagnostico));
        }


        const string Lorem =
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do " +
            "eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut " +
            "enim ad minim veniam, quis nostrud exercitation ullamco laboris " +
            "nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in " +
            "reprehenderit in voluptate velit esse cillum dolore eu fugiat " +
            "nulla pariatur. Excepteur sint occaecat cupidatat non proident, " +
            "sunt in culpa qui officia deserunt mollit anim id est laborum.\n";
    }
}
