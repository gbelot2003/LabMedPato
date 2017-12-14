//#define RenderMulti

using FormRender.Models;
using MCART;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
        Language lang;

        private void GetImg(LabeledImage j)
        {
            Image img = new Image { Source = j.imagen };
            TextBlock lbl = new TextBlock { Text = j.titulo };
            StackPanel pnl = new StackPanel { Children = { img, lbl } };
            BlockUIContainer bl = new BlockUIContainer(pnl);
            fltImages.Blocks.Add(bl);
        }

        internal FormPage(InformeResponse data, IEnumerable<LabeledImage> imgs, Size pgSize, Language language)
        {
            InitializeComponent();
            lang = language;
            switch (lang)
            {
                case FormRender.Language.Spanish:
                    lblTit.Text = "Reporte de Histopatología";
                    lblPac.Text = "Paciente:";
                    lblMed.Text = "Médico:";
                    lblAddr.Text = "Dirección:";
                    lblDiag.Text = "Diag. Clínico:";
                    lblMat.Text = "Material Estudiado:";
                    lblAge.Text = "Edad:";
                    lblSex.Text = "  Sexo:";
                    lblDte.Text = "Fecha:";
                    lblRec.Text = "Recibido:";
                    lblNB.Text = "No. biopsia:";
                    lblINF.Text = "INFORME";
                    lblIDt.Text = "Fecha de informe:";
                    break;
                case FormRender.Language.English:
                    lblTit.Text = "Histopathology Report";
                    lblPac.Text = "Patient:";
                    lblMed.Text = "Medic:";
                    lblAddr.Text = "Address:";
                    lblDiag.Text = "Clinical Diag.:";
                    lblMat.Text = "Studied Material:";
                    lblAge.Text = "Age:";
                    lblSex.Text = "  Sex:";
                    lblDte.Text = "Date:";
                    lblRec.Text = "Recieved:";
                    lblNB.Text = "Biopsy N.:";
                    lblINF.Text = "REPORT";
                    lblIDt.Text = "Report date:";
                    break;
                default:
                    break;
            }

            if (data.IsNull() || data.serial == 0) throw new ArgumentNullException();

            //Llenar header...
            txtPaciente.Text = data.facturas.nombre_completo_cliente;
            txtMedico.Text = data.facturas.medico;
            txtDireccion.Text = data.facturas.direccion_entrega_sede;
            txtDiag.Text = data.diagnostico;
            txtEstudiado.Text = data.muestra;
            txtEdad.Text = data.facturas.edad;
            txtSexo.Text = data.facturas.sexo;
            txtFecha.Text = data.fecha_biopcia.HasValue ? data.fecha_biopcia.Value.ToString("dd/MM/yyyy") : string.Empty;
            txtRecv.Text = data.fecha_muestra.HasValue ? data.fecha_muestra.Value.ToString("dd/MM/yyyy") : string.Empty;
            txtBiop.Text = $"{data.serial ?? 0} - {DateTime.Now.Year}";
            txtFactNum.Text = $"C.I. {data.factura_id}";
            txtFechaInf.Text = data.fecha_informe.HasValue ? data.fecha_informe.Value.ToString("dd/MM/yyyy") : string.Empty;

            // HACK: Parsear y extraer texto desde html...
            FlowDocument oo = XA.Parse(HTC.ConvertHtmlToXaml(
                data.informe

                /*
                 * Filtros de reemplazo
                 * ====================
                 * Solucionan los problemas de formateo con los q viene el
                 * texto desde el servidor. (la gente a veces no sabe redactar)
                 */
                .Replace("<div style=\"page-break-after:always\">", "") // remoción de <div /> separador innecesario
                .Replace("<br />\r\n&nbsp;<br />\r\n", "<br /><br />") // Sust. de nuevo párrafo (sucio a limpio)

                , true)) as FlowDocument;
            while (oo?.Blocks.Any() ?? false)
            {
                oo.Blocks.FirstBlock.FontFamily = FindResource("fntFmly") as FontFamily;
                oo.Blocks.FirstBlock.FontSize = (double)FindResource("fntSze");
                par.SiblingBlocks.Add(oo.Blocks.FirstBlock);
            }

            foreach (var j in imgs) GetImg(j);

            //Ajustar tamaño de columna...
            switch (data.images.Length)
            {
                case 0:
                    par.Inlines.Remove(fltImages);
                    break;
                case 1:
                case 2: break;
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
            docRoot.PageHeight = 720 - grdHead.ActualHeight;
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
            if (!dialog.ShowDialog() ?? true) return;
            var sz = new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight);
            Measure(sz);
            Arrange(new Rect(sz)); //ctrlSize
#if RenderMulti
            if (fdpwContent.PageCount == 1)
            {
                DoFirma(firma);
                DoFirma(firma2);
                txtPager.Text = $"Page 1/1 - Biopsia No. {txtBiop.Text}";
                fdpwContent.UpdateLayout();
                dialog.PrintVisual(this, $"Biopsia {txtBiop.Text}");
            }
            else
            {
                int c = 1;
                var pgSze = new Size(pageSize.Width * dpi, pageSize.Height * dpi);

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
#else
            for (int c = 1; c <= fdpwContent.PageCount; c++)
            {
                //Render compacto de una página
                if (c == fdpwContent.PageCount)
                {
                    DoFirma(firma);
                    DoFirma(firma2);
                }
                txtPager.Text = $"Page {c}/{fdpwContent.PageCount} - {lblNB.Text} {txtBiop.Text}";
                fdpwContent.UpdateLayout();
                dialog.PrintVisual(this, $"{lblNB.Text} {txtBiop.Text}");
                fdpwContent.NextPage();
            }
#endif
        }
    }
}