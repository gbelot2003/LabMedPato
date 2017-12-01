using FormRender.Models;
using FormRender.Pages;
using MCART;
using System;
using System.Collections.Generic;
using System.Windows;

namespace FormRender
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FormPage fr;
        public MainWindow()
        {
            InitializeComponent();

            var r = Utils.PatoClient.GetResponse(9865);

            Diagnostico diag = new Diagnostico
            {
                Texto = r.informe
            };
            const string Absolute = "http://192.168.2.101/img/histo/";
            List<LabeledImage> li = new List<LabeledImage>();
            foreach (var j in r.images)
            {
                li.Add(new LabeledImage
                {
                    RutaImagen = Absolute + j.image_url,
                    Titulo = j.descripcion
                });
            }

            diag.Imagenes = li.ToArray();

            HeaderInfo headerInfo = new HeaderInfo
            {
                Paciente = r.facturas.nombre_completo_cliente,
                Medico = r.facturas.medico,
                Direccion = r.facturas.direccion_entrega_sede,
                Diag = r.diagnostico,
                Estudiado = r.muestra,
                Edad = sbyte.Parse(r.facturas.edad.Substring(0, 2).Trim()),
                Sexo = char.Parse(r.facturas.sexo),
                Fecha = DateTime.Parse(r.fecha_informe),
                Recibido = DateTime.Now,//DateTime.Parse(r.fecha_biopsia),
                Biopsia = r.serial.Value.ToString()
            };
            fr = new FormPage(headerInfo, diag);
            
            fr.Measure(new Size(816, 1055));
            fr.Arrange(new Rect(new Size(816, 1055)));
            fr.UpdateLayout();

            frame.Source = fr.Render(new Size(2550, 3300), 300);
        }
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            fr.Print(new Size(2550, 3300));
        }
    }
}