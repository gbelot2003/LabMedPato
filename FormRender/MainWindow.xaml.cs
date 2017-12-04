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
        FormPage fr;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                fr = new FormPage(Utils.PatoClient.GetResponse(int.Parse(txtSerie.Text), int.Parse(txtfact.Text)), PageSizes.Carta);
                fr.Print();
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show("Serie o factura inválidos!","Error",MessageBoxButton.OK,MessageBoxImage.Exclamation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name,MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
    }
    public static class PageSizes
    {
        public static Size Carta => new Size(8.5, 11);
    }
}