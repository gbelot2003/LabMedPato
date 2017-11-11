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
using MCART;
using System.IO;

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
                //Texto = Lorem + Lorem + Lorem + Lorem + Lorem,
                Texto =
                @"<strong>DESCRIPCI&Oacute;N MACROSC&Oacute;PICA:</strong><br />
&nbsp;<br />
Se recibe formaci&oacute;n irregular, blanco rojizo de consistencia &oacute;sea,
mide 4x2.5x0.5cm, marcado con cr&oacute;mico largo el borde derecho, cr&oacute;mico
corto el borde izquierdo, seda corto borde posterior del paladar duro se ti&ntilde;e
con tinta china negra el cual es tejido oseo, y seda largo borde posterior tinta china
verde. Tinta china azul borde marcado con cr&oacute;mico largo, se incluye c&aacute;psula
A ( POSITIVO ), Se ti&ntilde;e con tinta china verde el borde marcado con seda largo,
Se incluye c&aacute;psula B (NEGATIVO ), se ti&ntilde;e con tinta rosada el borde
marcado con cr&oacute;mico corto el cual es tejido &oacute;seo. El borde opuesto a
la mucosa se ti&ntilde;e con tinta china negra, se incluye c&aacute;psula C, las partes
blandas (POSITIVO). Por separado se recibe m&uacute;ltiples fragmentos de tejido
&oacute;seo mide 2.5x2x2cm referido como borde adicional izquierdo.<br />
&nbsp;<br />
<strong>DESCRIPCI&Oacute;N MICROSC&Oacute;PICA:</strong><br />
&nbsp;<br />
&nbsp;<br />
<strong>DIAGN&Oacute;STICO:</strong>",
                RutaImagen = new LabeledImage[]
                {
                    new LabeledImage{
                        RutaImagen = @"C:\Users\xds_x\src\LabMedPato\FormRender\TestImages\Aqua.jpg",
                        Titulo = "Olas de agua"
                    },
                    new LabeledImage{
                        RutaImagen = @"C:\Users\xds_x\src\LabMedPato\FormRender\TestImages\Bricks2.png",
                        Titulo = "Ladrillos"
                    },
                    new LabeledImage{
                        RutaImagen = @"C:\Users\xds_x\src\LabMedPato\FormRender\TestImages\Grass.jpg",
                        Titulo = "Hierba"
                    }
                }
            };

            HeaderInfo headerInfo = new HeaderInfo
            {
                Paciente = "Juan Rodríguez",
                Medico = "Dr. José López",
                Direccion = "123 Algún lugar, Tgu",
                Diag = "Ejemplo",
                Estudiado = "Órganos",
                Edad = 25,
                Sexo = 'M',
                Fecha = DateTime.Today,
                Recibido = DateTime.Now,
                Biopsia = "123-4567-890123"
            };
            FormPage fr = new FormPage(headerInfo, diagnostico);

            PngBitmapEncoder pe = new PngBitmapEncoder();
            pe.Frames.Add(BitmapFrame.Create(fr.GetPage(new Size(2550, 3300))));
            using (FileStream file = File.OpenWrite("TestImage.png"))
            {
                pe.Save(file);
            }


            //fr.Print(new Size(2550, 3300));
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
