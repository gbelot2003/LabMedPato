using System;
using System.Globalization;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FormRender.Dialogs
{
    /// <summary>
    /// Interaction logic for LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog : Window
    {
        public LoginDialog()
        {
            InitializeComponent();
            bsyGo.SetBinding(VisibilityProperty, new Binding(nameof(Visibility)) { Source = btnGo, Converter = new VisibilityInverter() });
            btnGo.Click += BtnGo_Click;
            btnClose.Click += (sender, e) => Close();
            txtUsr.GotFocus += TxtFocus;
            txtPw.GotFocus += TxtFocus;
        }
        private void TxtFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox)?.SelectAll();
            (sender as PasswordBox)?.SelectAll();
        }
        private async void BtnGo_Click(object sender, RoutedEventArgs e)
        {
            loginWarning.Visibility = Visibility.Collapsed;
            btnGo.Visibility = Visibility.Hidden;
            if (!await Utils.PatoClient.Login(txtUsr.Text, txtPw.Password))
            {
                txtPw.Focus();
            }
            else
            {
                DialogResult = true;
                Close();
            }
            btnGo.Visibility = Visibility.Visible;
        }
        public bool GetLogin(out string usr, out SecureString password)
        {
            usr = string.Empty;
            return GetLogin(string.Empty, ref usr, out password);
        }
        /// <summary>
        /// Obtiene la información de inicio de sesión estableciendo valores
        /// predeterminados para los cuadros de texto de entrada.
        /// </summary>
        /// <param name="plainPw">Contraseña conocida.</param>
        /// <param name="usr">
        /// Parámetro de referencia. Nombre conocido del usuario.
        /// </param>
        /// <param name="password">Parámetro de salida. Contraseña.</param>
        /// <returns>
        /// <c>true</c> si se ha iniciado sesión correctamente, <c>false</c> en
        /// caso contrario.
        /// </returns>
        public bool GetLogin(string plainPw, ref string usr, out SecureString password)
        {
            txtUsr.Text = usr;
            txtPw.Password = plainPw;
            txtUsr.Focus();
            bool retVal = ShowDialog() ?? false;
            password = txtPw.SecurePassword;
            return retVal;
        }
    }

    public class VisibilityInverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility v = (Visibility)value;
            return v == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility v = (Visibility)value;
            return v == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}