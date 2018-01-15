//#define PagePrints
using FormRender.Pages;
using System.Windows;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace FormRender
{
    /// <summary>
    /// Lógica de interacción para PreviewWindows.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        FormPage page;
        int currpg = 1;
        bool updt;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="FormPage"/>.
        /// </summary>
        public PreviewWindow()
        {
            InitializeComponent();
            btnNext.Click += BtnNext_Click;
            btnPrev.Click += BtnPrev_Click;
            btnPrint.Click += BtnPrint_Click;
            sldTextSize.ValueChanged += SldTextSize_ValueChanged;
            sldImgWidth.ValueChanged += SldImgWidth_ValueChanged;
        }

        /// <summary>
        /// Muestra la vista previa de un informe.
        /// </summary>
        /// <param name="pg"></param>
        internal void ShowInforme(FormPage pg)
        {
            sldTextSize.Value = pg.TextSize;
            sldImgWidth.Value = pg.ImgSize;
            page = pg;
            page.View.LayoutUpdated += UpdtLayout;
            frmPreview.Navigate(pg);
            ShowDialog();
            page.View.LayoutUpdated -= UpdtLayout;
        }
        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (page.CanPrev)
            {
                currpg--;
                page.PrevPage();
            }
        }
        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (page.CanNext)
            {
                currpg++;
                page.NextPage();
            }
        }
        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
#if PagePrints
            page.GotoPage(1);
            page.LayoutUpdated -= UpdtLayout;
            page.Print();
            page.LayoutUpdated += UpdtLayout;
#else
            PrintDialog dialog = new PrintDialog();
            if (!dialog.ShowDialog() ?? true) return;
            var sz = new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight);
            //var sz2 = new Size(sz.Width * 788 / 96, sz.Height * 788 / 96);

            var document = new FixedDocument();
            document.DocumentPaginator.PageSize = sz;

            for (int c = 1; c <= page.PgCount; c++)
            {
                var p = new FormPage(page.Data, page.Imgs, sz, page.lang, false)
                {
                    ImgSize = page.ImgSize,
                    TextSize = page.TextSize
                };
                if (c == page.PgCount) p.DoFirmas();
                p.GotoPage(c);
                p.ShowPager(c, page.PgCount);
                p.Measure(sz);
                p.Arrange(new Rect(sz));
                p.UpdateLayout();
                Grid pc = p.RootContent;
                p.Content = null;
                p = null;

                pc.Measure(sz);
                pc.Arrange(new Rect(sz));
                pc.UpdateLayout();

                var fixedPage = new FixedPage
                {
                    Width = sz.Width,
                    Height = sz.Height
                };

                fixedPage.Children.Add(pc);
                var pageContent = new PageContent();
                ((IAddChild)pageContent).AddChild(fixedPage);
                document.Pages.Add(pageContent);
            }
            dialog.PrintDocument(document.DocumentPaginator, $"Biopsia {page.txtBiop.Text}");
#endif
        }
        private void SldImgWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!(page is null))
            {
                while (page.CanPrev)
                {
                    currpg--;
                    page.PrevPage();
                }
                page.ImgSize = e.NewValue;
                page.UndoFirma();
                if (!page.CanNext) page.DoFirmas();
            }
        }
        private void SldTextSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!(page is null))
            {
                while (page.CanPrev)
                {
                    currpg--;
                    page.PrevPage();
                }
                page.TextSize = e.NewValue;
                page.UndoFirma();
                if (!page.CanNext) page.DoFirmas();
            }
        }
        private async void UpdtLayout(object sender, EventArgs e)
        {
            if (!updt)
            {
                updt = true;
                await Task.Run(() =>
                {
                    Thread.Sleep(100);
                });
                int pc = page.PgCount;
                lblCounter.Text = $"Pág. {currpg}/{pc}";
                page.ShowPager(currpg);
                page.UndoFirma();
                if (currpg == pc)
                    page.DoFirmas();
                updt = false;
            }
        }
    }
}