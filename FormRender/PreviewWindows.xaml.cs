using FormRender.Models;
using FormRender.Pages;
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
using System.Windows.Shapes;
using HTC = HTMLConverter.HtmlToXamlConverter;
using XA = System.Windows.Markup.XamlReader;

namespace FormRender
{
    /// <summary>
    /// Lógica de interacción para PreviewWindows.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        FormPage page;
        int currpg = 1;

        public PreviewWindow()
        {
            InitializeComponent();
            btnNext.Click += BtnNext_Click;
            btnPrev.Click += BtnPrev_Click;
            btnPrint.Click += BtnPrint_Click;
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (page.CanPrev)
            {
                currpg--;
                lblCounter.Text = $"Pág. {currpg}/{page.PgCount}";
                page.PrevPage();
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (page.CanNext)
            {
                currpg++;
                lblCounter.Text = $"Pág. {currpg}/{page.PgCount}";
                page.NextPage();
            }
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            page.Print();
        }

        internal void ShowInforme(FormPage pg)
        {
            page = pg;
            frmPreview.Navigate(pg);
            lblCounter.Text = $"Pág. {currpg}/{page.PgCount}";
            ShowDialog();
        }
    }
}
