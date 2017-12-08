using FormRender.Models;
using FormRender.Pages;
using MCART;
using System;
using System.Collections.Generic;
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
        private async void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InformeResponse r;

                //await TaskbarItemInfo.Run(() => { });
                //r = Utils.PatoClient.GetResponse(int.Parse(txtSerie.Text), int.Parse(txtfact.Text), usr, pw);


                //(new FormPage(r, PageSizes.Carta)).Print();

                r = Utils.PatoClient.GetResponse(int.Parse(txtSerie.Text), int.Parse(txtfact.Text), usr, pw,"/eng");
                (new FormPage(r, PageSizes.Carta, true)).Print();
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