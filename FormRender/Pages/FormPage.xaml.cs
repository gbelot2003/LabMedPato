using FormRender.Models;
using TheXDS.MCART;
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
        const int DocSize = 870;

        FirmaResponse firma;
        FirmaResponse firma2;
        Size ctrlSize;
        string lblPg;
        Floater fltImages;
        double imgAdjust = 0;

        public readonly InformeResponse Data;
        public readonly IEnumerable<LabeledImage> Imgs;
        public readonly Size pageSize;
        public readonly Language lang;

        private void GetImg(LabeledImage j)
        {
            Image img = new Image { Source = j.imagen };
            TextBlock lbl = new TextBlock { Text = j.titulo, FontSize = 11 };
            StackPanel pnl = new StackPanel { Children = { img, lbl } };
            BlockUIContainer bl = new BlockUIContainer(pnl);
            fltImages.Blocks.Add(bl);
            Rsze();
            imgAdjust += pnl.ActualHeight + 80;
        }
        internal FormPage(InformeResponse data, IEnumerable<LabeledImage> imgs, Size pgSize, Language language, bool useDPI = true)
        {
            InitializeComponent();
            fltImages = FindResource("fltImages") as Floater;
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
                    lblSex.Text = "  Género:";
                    lblDte.Text = "Fecha:";
                    lblRec.Text = "Recibido:";
                    lblNB.Text = "No. biopsia:";
                    lblINF.Text = "INFORME";
                    lblIDt.Text = "Fecha de informe:";
                    lblPg = "Página";
                    break;
                case FormRender.Language.English:
                    lblTit.Text = "Histopathology Report";
                    lblPac.Text = "Patient:";
                    lblMed.Text = "Medic:";
                    lblAddr.Text = "Address:";
                    lblDiag.Text = "Clinical Diagnosis:";
                    lblMat.Text = "Speciment :";
                    lblAge.Text = "Age:";
                    lblSex.Text = "  Gender:";
                    lblDte.Text = "Date:";
                    lblRec.Text = "Received:";
                    lblNB.Text = "Biopsy N.:";
                    lblINF.Text = "REPORT";
                    lblIDt.Text = "Report date:";
                    lblPg = "Page";
                    break;
                default:
                    break;
            }

            if (data is null || data.serial == 0) throw new ArgumentNullException();

            Data = data;
            Imgs = imgs;
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
            txtBiop.Text = $"{data.serial ?? 0} - {data.fecha_muestra?.Year.ToString() ?? "N/A"}";
            txtFactNum.Text = $"C.I. {data.factura_id}";
            txtFechaInf.Text = data.fecha_informe.HasValue ? data.fecha_informe.Value.ToString("dd/MM/yyyy") : string.Empty;

            // Parsear y extraer texto desde html...
            FlowDocument oo = XA.Parse(HTC.ConvertHtmlToXaml(data.informe, true)) as FlowDocument;
            while (oo?.Blocks.Any() ?? false)
            {
                oo.Blocks.FirstBlock.FontFamily = FindResource("fntFmly") as FontFamily;
                oo.Blocks.FirstBlock.FontSize = (double)FindResource("fntSze");
                docRoot.Blocks.Add(oo.Blocks.FirstBlock);
            }

            if (imgs?.Any() ?? false)
            {
                var fb = docRoot.Blocks.FirstBlock as Paragraph;
                fb.Inlines.InsertBefore(fb.Inlines.FirstInline, fltImages);
            }

            pageSize = pgSize;
            if (useDPI)
                ctrlSize = new Size(pageSize.Width * UI.GetXDpi(), pageSize.Height * UI.GetYDpi());
            else
                ctrlSize = pgSize;

            Rsze();

            foreach (var j in imgs) GetImg(j);
            switch (data.images.Length)
            {
                case 0:
                case 1: break;
                default:
                    if (imgAdjust > DocSize - grdHead.ActualHeight)
                        fltImages.Width = 230 * ((DocSize) - grdHead.ActualHeight) / imgAdjust;
                    break;
            }

            RootContent.Width = ctrlSize.Width;
            RootContent.Height = ctrlSize.Height;
            UpdateLayout();

            firma = data.firma;
            firma2 = data.firma2;
            docRoot.PageHeight =
                ActualHeight
                - imgHeader.ActualHeight
                - grdHead.ActualHeight
                - grdFooter.ActualHeight
                - grdPager.ActualHeight;
        }
        public void Rsze()
        {
            Measure(ctrlSize);
            Arrange(new Rect(ctrlSize));
            UpdateLayout();
        }

        private void DoFirma(FirmaResponse f)
        {
            if (!(f is null))
            {
                StackPanel pnl = new StackPanel { Margin = new Thickness(60, 0, 0, 0) };
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
        public void DoFirmas()
        {
            DoFirma(firma);
            DoFirma(firma2);
        }
        public void UndoFirma() => grdFirmas.Children.Clear();
        public void NextPage() => fdpwContent.NextPage();
        public void PrevPage() => fdpwContent.PreviousPage();
        public void GotoPage(int page) => fdpwContent.GoToPage(page);
        public int PgCount => fdpwContent.PageCount;
        public bool CanNext => fdpwContent.CanGoToNextPage;
        public bool CanPrev => fdpwContent.CanGoToPreviousPage;
        public FlowDocumentPageViewer View => fdpwContent;
        public void ShowPager(int pgnum) => ShowPager(pgnum, fdpwContent.PageCount);
        public void ShowPager(int pgnum, int pgTot) => txtPager.Text = $"{lblPg} {pgnum}/{pgTot} - {lblNB.Text} {txtBiop.Text}";
        public double TextSize
        {
            get => docRoot.Blocks.FirstBlock.FontSize;
            set { foreach (var j in docRoot.Blocks) j.FontSize = value; }
        }
        public double ImgSize
        {
            get => fltImages.Width;
            set => fltImages.Width = value;
        }
    }
}