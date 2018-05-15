using FormRender.Models;
using TheXDS.MCART;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using HTC = HTMLConverter.HtmlToXamlConverter;
using XA = System.Windows.Markup.XamlReader;

namespace FormRender.Pages
{
    /// <inheritdoc cref="Page"/>
    /// <summary>
    /// Interaction logic for FormPage.xaml
    /// </summary>
    public partial class FormPage
    {
        private const int DocSize = 870;

        private readonly FirmaResponse _firma;
        private readonly FirmaResponse _firma2;
        private readonly Size _ctrlSize;
        private readonly string _lblPg;
        private readonly Floater _fltImages;
        private double _imgAdjust;

        public readonly InformeResponse Data;
        public readonly IEnumerable<LabeledImage> Imgs;
        private readonly Size _pageSize;
        public readonly Language Lang;

        private void GetImg(LabeledImage j)
        {
            var img = new Image { Source = j.imagen };
            var lbl = new TextBlock { Text = j.titulo, FontSize = 11 };
            var pnl = new StackPanel { Children = { img, lbl } };
            var bl = new BlockUIContainer(pnl);
            _fltImages.Blocks.Add(bl);
            Rsze();
            _imgAdjust += pnl.ActualHeight + 80;
        }
        internal FormPage(InformeResponse data, IEnumerable<LabeledImage> imgs, Size pgSize, Language language, bool useDpi = true)
        {
            InitializeComponent();

            if (data is null || data.serial == 0) throw new ArgumentNullException();

            _fltImages = (Floater) FindResource("FltImages");
            Lang = language;
            switch (Lang)
            {
                case FormRender.Language.Spanish:
                    LblTit.Text = "Reporte de Histopatología";
                    LblPac.Text = "Paciente:";
                    LblMed.Text = "Médico:";
                    LblAddr.Text = "Dirección:";
                    LblDiag.Text = "Diag. Clínico:";
                    LblMat.Text = "Material Estudiado:";
                    LblAge.Text = "Edad:";
                    LblSex.Text = "  Género:";
                    LblDte.Text = "Fecha:";
                    LblRec.Text = "Recibido:";
                    LblNb.Text = "No. biopsia:";
                    LblInf.Text = "INFORME";
                    LblIDt.Text = "Fecha de informe:";
                    _lblPg = "Página";

                    TxtFecha.Text = data.fecha_biopcia?.ToString("dd/MM/yyyy") ?? string.Empty;
                    TxtRecv.Text = data.fecha_muestra?.ToString("dd/MM/yyyy") ?? string.Empty;
                    TxtFechaInf.Text = data.fecha_informe?.ToString("dd/MM/yyyy") ?? string.Empty;

                    break;
                case FormRender.Language.English:
                    var ci = CultureInfo.CreateSpecificCulture("en-us");

                    LblTit.Text = "Histopathology Report";
                    LblPac.Text = "Patient:";
                    LblMed.Text = "Dr:";
                    LblAddr.Text = "Address:";
                    LblDiag.Text = "Clinical Diagnosis:";
                    LblMat.Text = "Specimen:";
                    LblAge.Text = "Age:";
                    LblSex.Text = "  Gender:";
                    LblDte.Text = "Date:";
                    LblRec.Text = "Received:";
                    LblNb.Text = "Biopsy N.:";
                    LblInf.Text = "REPORT";
                    LblIDt.Text = "Report date:";
                    _lblPg = "Page";

                    TxtFecha.Text = data.fecha_biopcia?.ToString("MMMM dd, yyyy",ci.DateTimeFormat) ?? string.Empty;
                    TxtRecv.Text = data.fecha_muestra?.ToString("MMMM dd, yyyy", ci.DateTimeFormat) ?? string.Empty;
                    TxtFechaInf.Text = data.fecha_informe?.ToString("MMMM dd, yyyy", ci.DateTimeFormat) ?? string.Empty;

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Data = data;
            var labeledImages = imgs.ToList();
            Imgs = labeledImages;

            //Llenar header...
            TxtPaciente.Text = data.facturas.nombre_completo_cliente;
            TxtMedico.Text = data.facturas.medico;
            TxtDireccion.Text = data.facturas.direccion_entrega_sede;
            TxtDiag.Text = data.diagnostico;
            TxtEstudiado.Text = data.muestra;
            TxtEdad.Text = data.facturas.edad;
            TxtSexo.Text = data.facturas.sexo;

            TxtBiop.Text = $"{data.serial ?? 0} - {data.fecha_muestra?.Year.ToString() ?? "N/A"}";
            TxtFactNum.Text = $"C.I. {data.factura_id}";

            // Parsear y extraer texto desde html...
            var oo = XA.Parse(HTC.ConvertHtmlToXaml(data.informe, true)) as FlowDocument;
            while (oo?.Blocks.Any() ?? false)
            {
                oo.Blocks.FirstBlock.FontFamily = (FontFamily) FindResource("FntFmly");
                oo.Blocks.FirstBlock.FontSize = (double)FindResource("FntSze");
                DocRoot.Blocks.Add(oo.Blocks.FirstBlock);
            }

            if (labeledImages.Any())
            {
                var fb = (Paragraph) DocRoot.Blocks.FirstBlock;
                fb.Inlines.InsertBefore(fb.Inlines.FirstInline, _fltImages);
            }

            _pageSize = pgSize;
            _ctrlSize = useDpi ? new Size(_pageSize.Width * UI.GetXDpi(), _pageSize.Height * UI.GetYDpi()) : pgSize;

            Rsze();

            foreach (var j in labeledImages) GetImg(j);
            switch (data.images.Length)
            {
                case 0:
                case 1: break;
                default:
                    if (_imgAdjust > DocSize - GrdHead.ActualHeight)
                        _fltImages.Width = 230 * ((DocSize) - GrdHead.ActualHeight) / _imgAdjust;
                    break;
            }

            RootContent.Width = _ctrlSize.Width;
            RootContent.Height = _ctrlSize.Height;
            UpdateLayout();

            _firma = data.firma;
            _firma2 = data.firma2;
            DocRoot.PageHeight =
                ActualHeight
                - ImgHeader.ActualHeight
                - GrdHead.ActualHeight
                - GrdFooter.ActualHeight
                - LblTit.ActualHeight
                - LblInf.ActualHeight
                - 25
                - GrdPager.ActualHeight;
        }

        private void Rsze()
        {
            Measure(_ctrlSize);
            Arrange(new Rect(_ctrlSize));
            UpdateLayout();
        }

        private void DoFirma(FirmaResponse f)
        {
            if (f is null) return;
            var pnl = new StackPanel { Margin = new Thickness(60, 0, 0, 0) };
            if (!f.name.IsEmpty())
            {
                var t = new TextBlock
                {
                    Text = f.name,
                    TextAlignment = TextAlignment.Center,
                    FontWeight = FontWeights.Bold
                };
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
            GrdFirmas.Children.Add(pnl);
        }
        public void DoFirmas()
        {
            DoFirma(_firma);
            DoFirma(_firma2);
        }
        public void UndoFirma() => GrdFirmas.Children.Clear();
        public void NextPage() => FdpwContent.NextPage();
        public void PrevPage() => FdpwContent.PreviousPage();
        public void GotoPage(int page) => FdpwContent.GoToPage(page);
        public int PgCount => FdpwContent.PageCount;
        public bool CanNext => FdpwContent.CanGoToNextPage;
        public bool CanPrev => FdpwContent.CanGoToPreviousPage;
        public FlowDocumentPageViewer View => FdpwContent;
        public void ShowPager(int pgnum) => ShowPager(pgnum, FdpwContent.PageCount);
        public void ShowPager(int pgnum, int pgTot) => TxtPager.Text = $"{_lblPg} {pgnum}/{pgTot} - {LblNb.Text} {TxtBiop.Text}";
        public double TextSize
        {
            get => DocRoot.Blocks.FirstBlock.FontSize;
            set { foreach (var j in DocRoot.Blocks) j.FontSize = value; }
        }
        public double ImgSize
        {
            get => _fltImages.Width;
            set => _fltImages.Width = value;
        }
        public void HeadFoot(bool show)
        {
            if (show)
            {
                ImgHeader.Visibility = Visibility.Visible;
                GrdFooter.Visibility = Visibility.Visible;
            }
            else
            {
                ImgHeader.Visibility = Visibility.Hidden;
                GrdFooter.Visibility = Visibility.Hidden;
            }
        }
    }
}