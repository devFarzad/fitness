using fitness.Views.Login;
using System.Threading;
using System.Windows;

namespace fitness
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var langCode = fitness.Properties.Settings.Default.languageCode;
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(langCode);
            base.OnStartup(e);

        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Login WindowLogin = new Login
            {

            };
            WindowLogin.ShowDialog();

            //MainWindow WindowMain = new MainWindow
            //{

            //};
            //WindowMain.Show();

        }

    }
}
