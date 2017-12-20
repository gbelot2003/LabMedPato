using FormRender.Models;
using FormRender.Pages;
using MCART;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

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
            stckStatus.SetBinding(VisibilityProperty, new Binding(nameof(IsEnabled))
            {
                Source = pnlControls,
                Converter = new System.Windows.Converters.BooleanToInvVisibilityConverter()
            });
            btnPrint.Tag = new ApiInfo { ruta = null, language = FormRender.Language.Spanish };
            btnPrint2.Tag = new ApiInfo { ruta = "/eng", language = FormRender.Language.English };
            var pwD = new MCART.Forms.PasswordDialog();
            try
            {
                var r = pwD.Login(usr, pw, (u, p) => Utils.PatoClient.Login(u, p.ReadString()));
                if (r.Result == MessageBoxResult.OK)
                {
                    usr = r.Usr;
                    pw = r.Pwd.ReadString();
                    queueClose = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally { pwD.Dispose(); }
            Loaded += (sender, e) => { if (queueClose) Close(); };
        }

        private async void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = ((sender as Button)?.Tag as ApiInfo) ?? throw new Exception("El control no es un botón o no contiene información de API");
                int serie = int.Parse(txtSerie.Text), fact = int.Parse(txtfact.Text);
                var resp = Utils.PatoClient.GetResponse(serie, fact, usr, pw, btn.ruta);
                pnlControls.IsEnabled = false;
                List<LabeledImage> imgs = new List<LabeledImage>();
                int c = 0;
                foreach (var j in resp.images)
                {
                    lblStatus.Text = $"Descargando {j.descripcion ?? "imagen"}...";
                    pgbStatus.Value = (c * 100 / resp.images.Length);
                    var ms = new System.IO.MemoryStream();
                    await MCART.Networking.Misc.DownloadHttpAsync(new Uri(Misc.imgPath + j.image_url), ms);
                    imgs.Add(new LabeledImage
                    {
                        imagen = UI.GetBitmap(ms),
                        titulo = j.descripcion
                    });
                    c++;
                }
                pgbStatus.Value = 100;
                lblStatus.Text = "Generando informe...";
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
                    .Replace("<br/><br/><br/><br/>", "<br/><br/>")
                    .Replace("</strong><br/><br/>", "</strong><br />");

                // Prueba para fingir contenidos largos
                resp.informe += resp.informe;

                (new PreviewWindow()).ShowInforme(new FormPage(resp, imgs, PageSizes.Carta, btn.language));
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Serie o factura inválidos!", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n{ex.StackTrace}", ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                lblStatus.Text = null;
                pnlControls.IsEnabled = true;
            }
        }
    }
}