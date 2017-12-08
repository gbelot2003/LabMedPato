using FormRender.Models;
using FormRender.Pages;
using MCART;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace FormRender
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string usr, pw;
        public MainWindow()
        {
            InitializeComponent();
            MCART.Forms.PasswordDialog pwD = new MCART.Forms.PasswordDialog();
            var r = pwD.GetPassword(null, null, true);
            usr = r.Usr;
            pw = r.Pwd;
        }

        private void btnPrint2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pnlControls.IsEnabled = false;
                lblStatus.Text = "Obteniendo informe, por favor, espere...";
                (new FormPage(Utils.PatoClient.GetResponse(int.Parse(txtSerie.Text), int.Parse(txtfact.Text), usr, pw, "/eng"), PageSizes.Carta, true)).Print();
                lblStatus.Text = null;
                pnlControls.IsEnabled = true;
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Serie o factura inválidos!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pnlControls.IsEnabled = false;
                lblStatus.Text = "Obteniendo informe, por favor, espere...";
                int serie = int.Parse(txtSerie.Text), fact = int.Parse(txtfact.Text);
                UpdateLayout();
                await Task.Run(() => { });
                (new FormPage(Utils.PatoClient.GetResponse(serie, fact, usr, pw), PageSizes.Carta)).Print();
                lblStatus.Text = null;
                pnlControls.IsEnabled = true;
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Serie o factura inválidos!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    public static class PageSizes
    {
        public static Size Carta => new Size(8.5, 11);
    }
}