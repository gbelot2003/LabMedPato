using System;
using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace FormRender.Dialogs
{
    /// <summary>
    /// Interaction logic for LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog : Window
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="LoginDialog"/>.
        /// </summary>
        public LoginDialog()
        {
            InitializeComponent();
            BtnGo.Click += BtnGo_Click;
            BtnClose.Click += (sender, e) => Close();
            TxtUsr.GotFocus += TxtFocus;
            TxtUsr.TextChanged += WarnClear;
            TxtPw.GotFocus += TxtFocus;
            TxtPw.PasswordChanged += WarnClear;
        }
        private void WarnClear(object sender, RoutedEventArgs e)
        {
            LoginWarning.Text = string.Empty;
        }
        private void TxtFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox)?.SelectAll();
            (sender as PasswordBox)?.SelectAll();
        }
        private async void BtnGo_Click(object sender, RoutedEventArgs e)
        {
            BtnGo.IsEnabled = false;
            try
            {
                if (!await Utils.PatoClient.Login(TxtUsr.Text, TxtPw.Password))
                {
                    LoginWarning.Text = "Contraseña inválida.";
                    TxtPw.Focus();
                }
                else
                {
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                LoginWarning.Text = ex.Message;
            }
            finally
            {
                BtnGo.IsEnabled = true;
            }
        }

        /// <summary>
        /// Obtiene la información de inicio de sesión.
        /// </summary>
        /// <param name="usr">Usuario</param>
        /// <param name="password">Contraseña</param>
        /// <returns>
        /// <c>true</c> si se ha iniciado sesión correctamente, <c>false</c> en
        /// caso contrario.
        ///</returns>
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
            TxtUsr.Text = usr;
            TxtPw.Password = plainPw;
            TxtUsr.Focus();
            bool retVal = ShowDialog() ?? false;
            usr = TxtUsr.Text;
            password = TxtPw.SecurePassword;
            return retVal;
        }
    }
}