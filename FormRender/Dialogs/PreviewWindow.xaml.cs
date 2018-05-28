using FormRender.Pages;
using System.Windows;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using FormRender.Utils;
using System.Windows.Data;

namespace FormRender.Dialogs
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
            BtnNext.Click += BtnNext_Click;
            BtnPrev.Click += BtnPrev_Click;
            BtnPrint.Click += BtnPrint_Click;
            BtnWord.Click += BtnWord_Click;
            SldTextSize.ValueChanged += SldTextSize_ValueChanged;
            SldImgWidth.ValueChanged += SldImgWidth_ValueChanged;
            BusyMessage.SetBinding(VisibilityProperty, new Binding(nameof(IsEnabled))
            {
                Source = PnlControls,
                Converter = new System.Windows.Converters.BooleanToInvVisibilityConverter()
            });
        }

        /// <summary>
        /// Muestra la vista previa de un informe.
        /// </summary>
        /// <param name="pg"></param>
        /// <param name="withHeader"></param>
        public void ShowInforme(FormPage pg, bool withHeader = true)
        {
            SldTextSize.Value = pg.TextSize;
            SldImgWidth.Value = pg.ImgSize;
            page = pg;
            ChkConHeader.IsChecked = withHeader;
            page.View.LayoutUpdated += UpdtLayout;
            FrmPreview.Navigate(pg);
            ShowDialog();
            page.View.LayoutUpdated -= UpdtLayout;
        }

        #region Botones y controles
        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog dialog = new PrintDialog();
            if (!dialog.ShowDialog() ?? true) return;
            var sz = new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight);
            var document = new FixedDocument();
            document.DocumentPaginator.PageSize = sz;
            for (int c = 1; c <= page.PgCount; c++)
            {
                var p = new FormPage(page.Data, page.Imgs, sz, page.Lang, false)
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
                p.FdpwContent.UpdateLayout();
                if (c == page.PgCount)
                    MessageBox.Show("Imprimiendo documento...", "Imprimir", MessageBoxButton.OK, MessageBoxImage.Information);
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
            dialog.PrintDocument(document.DocumentPaginator, $"Biopsia {page.TxtBiop.Text}");
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
            if (page is null) return;
            while (page.CanPrev)
            {
                currpg--;
                page.PrevPage();
            }
            page.TextSize = e.NewValue;
            page.UndoFirma();
            if (!page.CanNext) page.DoFirmas();
        }
        private async void BtnWord_Click(object sender, RoutedEventArgs e)
        {
            PnlControls.IsEnabled = false;
            try
            {
                await Task.Run(() =>
                {
                    var w = new WordInterop();
                    w.Convert(page.Data, page.Imgs, page.Lang);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n-----------------\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                PnlControls.IsEnabled = true;
            }
        }
        #endregion

        private async void UpdtLayout(object sender, EventArgs e)
        {
            if (!updt)
            {
                updt = true;
                BtnPrev.IsEnabled = page.CanPrev;
                BtnNext.IsEnabled = page.CanNext;

                // Necesario, libera al CPU de constantes llamadas de Layout.
                await Task.Run(() =>
                {
                    Thread.Sleep(100);
                });
                int pc = page.PgCount;
                LblCounter.Text = $"Pág. {currpg}/{pc}";
                page.ShowPager(currpg);
                page.UndoFirma();
                if (currpg == pc)
                    page.DoFirmas();
                updt = false;
            }
        }

        private void ChkConHeader_OnChecked(object sender, RoutedEventArgs e)
        {
            page?.HeadFoot(true);
        }

        private void ChkConHeader_OnUnchecked(object sender, RoutedEventArgs e)
        {
            page?.HeadFoot(false);
        }
    }
}