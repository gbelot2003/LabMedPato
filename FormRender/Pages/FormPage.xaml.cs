using FormRender.Models;
using MCART;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using HTC = HTMLConverter.HtmlToXamlConverter;
using XA = System.Windows.Markup.XamlReader;

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

            //txtCortado.SetBinding(WidthProperty, new Binding("ActualWidth") { Source = grdWidth });

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

            // HACK: Parsear y extraer texto desde html...
            FlowDocument oo = XA.Parse(HTC.ConvertHtmlToXaml(diag.Texto, true)) as FlowDocument;
            while (oo?.Blocks.Any() ?? false)
            {
                oo.Blocks.FirstBlock.FontFamily = FindResource("fntFmly") as FontFamily;
                oo.Blocks.FirstBlock.FontSize = (double)FindResource("fntSze");
                par.SiblingBlocks.Add(oo.Blocks.FirstBlock);
            }

            foreach (var j in diag.Imagenes)
            {
                Image img = new Image { Source = UI.GetImageHttp(j.RutaImagen) };
                TextBlock lbl = new TextBlock { Text = j.Titulo };
                StackPanel pnl = new StackPanel { Children = { img, lbl } };
                BlockUIContainer bl = new BlockUIContainer(pnl);
                fltImages.Blocks.Add(bl);
            }

            //Ajustar tamaño de columna...
            switch (diag.Imagenes.Length)
            {
                case 0:
                    par.Inlines.Remove(fltImages);
                    break;
                case 1: break;
                case 2:
                    fltImages.Width = 150;
                    break;
                default:
                    fltImages.Width = 150;
                    break;
            }
        }

        public void Print(Size pageSize, short dpi = 300)
        {
            Size ctrlSze = new Size(pageSize.Width * 96 / dpi, pageSize.Height * 96 / 300);
            PrintDialog dialog = new PrintDialog();
            Measure(ctrlSze);
            Arrange(new Rect(ctrlSze));
            //UpdateLayout();
            if (fdpwContent.PageCount == 1)
            {
                //Render compacto de una página
                fdpwContent.UpdateLayout();
                dialog.PrintVisual(this, $"Biopsia {txtBiop.Text}");
            }
            else
            {
                //HACK: Las páginas deben renderizarse como bitmaps antes de imprimirse...
                var document = new FixedDocument();
                document.DocumentPaginator.PageSize = pageSize;
                for (int j = 0; j < fdpwContent.PageCount; j++)
                {
                    var fixedPage = new FixedPage
                    {
                        Width = ctrlSze.Width,
                        Height = ctrlSze.Height
                    };
                    fdpwContent.UpdateLayout();
                    fixedPage.Children.Add(new Image { Source = this.Render(ctrlSze, pageSize, 300) });
                    var pageContent = new PageContent();
                    ((IAddChild)pageContent).AddChild(fixedPage);
                    document.Pages.Add(pageContent);
                    fdpwContent.NextPage();
                }
                dialog.PrintDocument(document.DocumentPaginator, $"Biopsia {txtBiop.Text}");
            }
        }
    }
}