using FormRender.Dialogs;
using FormRender.Models;
using FormRender.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TheXDS.MCART;
using static TheXDS.MCART.Networking.DownloadHelper;

namespace FormRender
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private class ApiInfo
        {
            internal string ruta;
            internal Language language;
        }

        bool queueClose = true;
        string usr = string.Empty, pw = string.Empty;
        public MainWindow()
        {
            InitializeComponent();
            StckStatus.SetBinding(VisibilityProperty, new Binding(nameof(IsEnabled))
            {
                Source = PnlControls,
                Converter = new System.Windows.Converters.BooleanToInvVisibilityConverter()
            });
            BtnPrint.Tag = new ApiInfo { ruta = null, language = FormRender.Language.Spanish };
            BtnPrint2.Tag = new ApiInfo { ruta = "/eng", language = FormRender.Language.English };
#if DEBUG
            usr = "gbelot";
            pw = "Luna0102";
            TxtSerie.Text = "12463";
            Txtfact.Text = "5129147";
#endif
            try
            {
                queueClose = !new Dialogs.LoginDialog().GetLogin(pw, ref usr, out var spw);
                pw = spw.Read();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Loaded += (sender, e) => { if (queueClose) Close(); };
            TxtSerie.GotFocus += Txts_GotFocus;
            Txtfact.GotFocus += Txts_GotFocus;
        }

        private void Txts_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox)?.SelectAll();
        }

        private async void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PnlControls.IsEnabled = false;
                PgbStatus.IsIndeterminate = true;
                LblStatus.Text = "Obteniendo informe...";
                var btn = (sender as Button)?.Tag as ApiInfo ?? throw new Exception("El control no es un botón o no contiene información de API");
                var resp = await Utils.PatoClient.GetResponse(int.Parse(TxtSerie.Text), int.Parse(Txtfact.Text), usr, pw, btn.ruta);
                if (resp is null)
                {
                    MessageBox.Show("Serie o factura inválidos!", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                PgbStatus.IsIndeterminate = false;

                List<LabeledImage> imgs = new List<LabeledImage>();
                int c = 1;
                foreach (var j in resp.images)
                {
                    PgbStatus.Value = 0;
                    LblStatus.Text = $"Descargando {j.descripcion ?? "imagen"} ({c}/{resp.images.Length})...";
                    var ms = new System.IO.MemoryStream();
                    await DownloadHttpAsync(new Uri(Config.imgPath + j.image_url), ms, (p, t) =>
                    {
                        PgbStatus.Dispatcher.Invoke(() =>
                        {
                            if (p.HasValue)
                            {
                                PgbStatus.IsIndeterminate = false;
                                PgbStatus.Value = (long) p * 100 / t;
                            }
                            else
                            {
                                PgbStatus.IsIndeterminate = true;
                            }
                        });
                    });
                    imgs.Add(new LabeledImage
                    {
                        imagen = UI.GetBitmap(ms),
                        titulo = j.descripcion
                    });
                    c++;
                }
                PgbStatus.Value = 100;
                LblStatus.Text = "Generando informe...";
                resp.informe = resp.informe
                    /*
                     * Filtros de reemplazo
                     * ====================
                     * Solucionan los problemas de formateo con los q viene el
                     * texto desde el servidor. (la gente a veces no sabe redactar)
                     */
                    .Replace("<br />", "<br/>")                             // Preproceso de breaks
                    .Replace("<div style=\"page-break-after:always\">", "") // remoción de <div /> separador innecesario
                    .Replace("<br/>\r\n&nbsp;<br/>\r\n", "<br/><br/>")      // Sust. de nuevo párrafo (sucio a limpio)
                    .Replace("\r\n", "<br/>")                               // Sust. de caracteres \r\n a <br/>
                    .Replace("\n", "<br/>")                                 // Sust. de caracter \n a <br/>
                    .Replace("<br/><br/><br/><br/>", "<br/><br/>")          // Remoción de párrafos innecesarios
                    .Replace("</strong><br/><br/>", "</strong><br/>");      // Remoción de cambio de párrafo después de título

                new PreviewWindow().ShowInforme(new FormPage(resp, imgs, PageSizes.Carta, btn.language), resp.images.Any());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n{ex.StackTrace}", ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LblStatus.Text = null;
                PnlControls.IsEnabled = true;
            }
        }
    }
}