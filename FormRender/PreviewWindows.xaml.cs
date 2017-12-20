using FormRender.Pages;
using System.Windows;

namespace FormRender
{
    /// <summary>
    /// Lógica de interacción para PreviewWindows.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        FormPage page;
        int currpg = 1;
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="FormPage"/>.
        /// </summary>
        public PreviewWindow()
        {
            InitializeComponent();
            btnNext.Click += BtnNext_Click;
            btnPrev.Click += BtnPrev_Click;
            btnPrint.Click += BtnPrint_Click;
        }

        /// <summary>
        /// Muestra la vista previa de un informe.
        /// </summary>
        /// <param name="pg"></param>
        internal void ShowInforme(FormPage pg)
        {
            page = pg;
            page.LayoutUpdated += (sender, e) => lblCounter.Text = $"Pág. {currpg}/{page.PgCount}";
            frmPreview.Navigate(pg);
            page.ShowPager(currpg);
            if (!page.CanNext) page.DoFirmas();
            ShowDialog();
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (page.CanPrev)
            {
                currpg--;
                lblCounter.Text = $"Pág. {currpg}/{page.PgCount}";
                page.PrevPage();
                page.ShowPager(currpg);
                if (page.CanNext) page.UndoFirma();

            }
        }
        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (page.CanNext)
            {
                currpg++;
                lblCounter.Text = $"Pág. {currpg}/{page.PgCount}";
                page.NextPage();
                page.ShowPager(currpg);
                if (!page.CanNext) page.DoFirmas();
            }
        }
        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            page.Print();
        }
   }
}