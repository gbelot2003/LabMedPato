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
using System.Drawing;

namespace FormRender.Pages
{
    /// <summary>
    /// Interaction logic for FormPage.xaml
    /// </summary>
    public partial class FormPage : Page
    {
        public FormPage(HeaderInfo header, Diagnostico diag)
        {
            InitializeComponent();

            //Llenar header...
            txtPaciente.Text = header.Paciente;
            txtMedico.Text = header.Medico;
            txtDireccion.Text = header.Direccion;
            txtDiag.Text = header.Diag;
            txtEstudiado.Text = header.Estudiado;
            txtEdad.Text = header.Edad.ToString();
            txtSexo.Text = header.Sexo.ToString();
            txtFecha.Text = header.Fecha.ToString();
            txtRecv.Text = header.Recibido.ToString();
            txtBiop.Text = header.Biopsia;

            //Crear informe...
            txtCortado.Text = diag.TextoCortado;
            txtCompleto.Text = diag.TextoResto;

            foreach (var j in diag.RutaImagen) 
            {                
                BitmapImage i = new BitmapImage(new Uri(j));
                Image img = new Image { Source = i };
                pnlImagenes.Children.Add(img);
            }

            //Ajustar tamaño de columna...
            //switch (diag.RutaImagen.Length)
            //{
                //case 2:
            //}
        }


    }
}
