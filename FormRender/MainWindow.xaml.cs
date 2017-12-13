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

        string usr, pw;
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

            MCART.Forms.PasswordDialog pwD = new MCART.Forms.PasswordDialog();
            var r = pwD.GetPassword(null, null, true);
            if (r.Result == MessageBoxResult.OK)
            {
                usr = r.Usr;
                pw = r.Pwd.ReadString();
            }
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
                (new FormPage(resp, imgs, PageSizes.Carta, btn.language)).Print();
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Serie o factura inválidos!", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                lblStatus.Text = null;
                pnlControls.IsEnabled = true;
            }
        }
    }
}