using FormRender.Models;
using MCART;
using System;
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
        FirmaResponse firma;
        FirmaResponse firma2;
        Size pageSize;
        Size ctrlSize;
        private const string imgPath = "http://192.168.2.101/img/histo/";
        public FormPage(InformeResponse data, Size pgSize)
        {
            InitializeComponent();

            //Llenar header...
            txtPaciente.Text = data.facturas.nombre_completo_cliente;
            txtMedico.Text = data.facturas.medico;
            txtDireccion.Text = data.facturas.direccion_entrega_sede;
            txtDiag.Text = data.diagnostico;
            txtEstudiado.Text = data.muestra;
            txtEdad.Text = data.facturas.edad;
            txtSexo.Text = data.facturas.sexo;
            txtFecha.Text = data.fecha_biopcia.ToString();
            txtRecv.Text = data.fecha_muestra.ToString();
            txtBiop.Text = $"{data.serial ?? 0} - {DateTime.Now.Year}";
            txtFactNum.Text = $"C.I. {data.factura_id}";
            txtFechaInf.Text = data.fecha_informe.ToString();

            // HACK: Parsear y extraer texto desde html...
            FlowDocument oo = XA.Parse(HTC.ConvertHtmlToXaml(data.informe, true)) as FlowDocument;
            while (oo?.Blocks.Any() ?? false)
            {
                oo.Blocks.FirstBlock.FontFamily = FindResource("fntFmly") as FontFamily;
                oo.Blocks.FirstBlock.FontSize = (double)FindResource("fntSze");
                par.SiblingBlocks.Add(oo.Blocks.FirstBlock);
            }

            foreach (var j in data.images)
            {
                Image img = new Image { Source = UI.GetImageHttp(imgPath + j.image_url) };
                TextBlock lbl = new TextBlock { Text = j.descripcion };
                StackPanel pnl = new StackPanel { Children = { img, lbl } };
                BlockUIContainer bl = new BlockUIContainer(pnl);
                fltImages.Blocks.Add(bl);
            }

            //Ajustar tamaño de columna...
            switch (data.images.Length)
            {
                case 0:
                    par.Inlines.Remove(fltImages);
                    break;
                case 1: break;
                default:
                    fltImages.Width = 150;
                    break;
            }
            firma = data.firma;
            firma2 = data.firma2;
            pageSize = pgSize;
            ctrlSize = new Size(pageSize.Width * UI.GetXDpi(), pageSize.Height * UI.GetYDpi());
            Measure(ctrlSize);
            Arrange(new Rect(ctrlSize));
            UpdateLayout();
        }

        private void DoFirma(FirmaResponse f)
        {
            if (!f.IsNull())
            {
                grdFirmas.ColumnDefinitions.Add(new ColumnDefinition());
                StackPanel pnl = new StackPanel();
                Grid.SetColumn(pnl, grdFirmas.ColumnDefinitions.Count - 1);

                if (!f.name.IsEmpty())
                {
                    TextBlock t = new TextBlock
                    {
                        Text = f.name,
                        TextAlignment = TextAlignment.Center
                    };
                    t.FontWeight = FontWeights.Bold;
                    pnl.Children.Add(t);
                }
                if (!f.collegiate.IsEmpty())
                {
                    pnl.Children.Add(new TextBlock
                    {
                        Text = f.collegiate,
                        TextAlignment = TextAlignment.Center
                    });
                }
                if (!f.extra.IsEmpty())
                {
                    pnl.Children.Add(new TextBlock
                    {
                        Text = f.extra,
                        TextAlignment = TextAlignment.Center
                    });
                }
                grdFirmas.Children.Add(pnl);
            }
        }

        public void Print(short dpi = 300)
        {
            PrintDialog dialog = new PrintDialog();
            Measure(ctrlSize);
            Arrange(new Rect(ctrlSize));
            int c = 1;

            var pgSze = new Size(pageSize.Width * dpi, pageSize.Height * dpi);


            if (fdpwContent.PageCount == 1)
            {
                //Render compacto de una página
                txtPager.Text = $"Page 1/1 - Biopsia No. {txtBiop.Text}";
                fdpwContent.UpdateLayout();
                dialog.PrintVisual(this, $"Biopsia {txtBiop.Text}");
            }
            else
            {

                //HACK: Las páginas deben renderizarse como bitmaps antes de imprimirse...
                var document = new FixedDocument();
                document.DocumentPaginator.PageSize = pgSze;
                for (int j = 0; j < fdpwContent.PageCount; j++)
                {
                    txtPager.Text = $"Page {c}/{fdpwContent.PageCount} - Biopsia No. {txtBiop.Text}";
                    if (c >= fdpwContent.PageCount)
                    {
                        DoFirma(firma);
                        DoFirma(firma2);
                    }
                    var fixedPage = new FixedPage
                    {
                        Width = ctrlSize.Width,
                        Height = ctrlSize.Height
                    };
                    fdpwContent.UpdateLayout();
                    fixedPage.Children.Add(new Image { Source = this.Render(ctrlSize, pgSze, dpi) });
                    var pageContent = new PageContent();
                    ((IAddChild)pageContent).AddChild(fixedPage);
                    document.Pages.Add(pageContent);
                    c++;
                    fdpwContent.NextPage();
                }
                dialog.PrintDocument(document.DocumentPaginator, $"Biopsia {txtBiop.Text}");
            }
        }
    }
}