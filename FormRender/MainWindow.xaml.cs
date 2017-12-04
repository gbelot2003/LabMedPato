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

            fr = new FormPage(Utils.PatoClient.GetResponse(9865), PageSizes.Carta);
        }
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            fr.Print();
        }
    }
    public static class PageSizes
    {
        public static Size Carta => new Size(8.5, 11);
    }
}