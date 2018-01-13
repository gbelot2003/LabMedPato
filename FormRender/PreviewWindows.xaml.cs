using FormRender.Pages;
using System.Windows;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

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
            page.Print();
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
                if (currpg == pc)//!page.CanNext)
                    page.DoFirmas();
                updt = false;
            }
        }
    }
}