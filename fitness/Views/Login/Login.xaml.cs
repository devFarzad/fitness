using DataModelFitness;
using fitness.Model;
using fitness.Views.MainPage;
using fitness.Views.ManageConnectionString;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
namespace fitness.Views.Login
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }
        internal RegisterData Registery = new RegisterData();
        private void cmb_Lang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void PanelMove_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void LoadData() // Load All Data For LoginPage
        {
            txt_UserName.Text = Registery.LoadDataOnRegistry();
            string getConnectionString = "";
            if (Registery.LoadConnectionString(getConnectionString))
            {
                Btn_Login.IsEnabled = true;
            }
            else
            {
                Btn_Login.IsEnabled = false;
                if (MessageBox.Show("Connection Error ,are you repair connection string? ",
                                   "Error",
                                   MessageBoxButton.YesNo,
                                   MessageBoxImage.Error) ==
                    MessageBoxResult.Yes)
                {
                    SettingsDB SettingWindow = new SettingsDB { };
                    SettingWindow.ShowDialog();
                    Btn_Login.IsEnabled = true;
                }
            }
        }

        public string PasswordHash(string password) // this Function for Hashing Password
        {
            // Password Hashing //



            SHA256CryptoServiceProvider sha2 = new SHA256CryptoServiceProvider();
            Byte[] arrB1;
            Byte[] arrB2;
            arrB1 = UTF8Encoding.UTF8.GetBytes(password);
            arrB2 = sha2.ComputeHash(arrB1);
            string passwordHash = BitConverter.ToString(arrB2);

            // End Password Hashing 
            return passwordHash;
        }

        private bool CheckAuth(string userName, string password)
        {
            try
            {
                using (fitnessEntities db = new fitnessEntities(dbConnection.ConnectionString))
                {
                    // linq query

                    var querySelectUser = from varUser in db.vw_AuthUsers
                                          where varUser.user_Auth_UserName == userName
                                          where varUser.user_Auth_Password == password
                                          where varUser.user_Auth_Status == 1
                                          select varUser;
                    var resultSelectUser = querySelectUser.ToList();
                    if (resultSelectUser.Count == 1)
                    {
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("User Not Found !");
                        return false;
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : \n\n\n " + ex.ToString());
            }
            return false;
        }
        private bool checkNullable()
        {
            if (string.IsNullOrEmpty(txt_UserName.Text.Trim()))
            {
                MessageBox.Show("Please Enter UserName");
                return false;
            }
            else if (string.IsNullOrEmpty(pw_Password.Password.Trim()))
            {
                MessageBox.Show("Please Enter Password");
                return false;
            }
            return true;
        }
        private void Btn_Login_Click(object sender, RoutedEventArgs e)
        {
            if (!checkNullable())
            {
                return;
            }
            else
            {
                if (CheckAuth(txt_UserName.Text.Trim().ToLower(), PasswordHash(pw_Password.Password)))
                {
                    Main ShowMain = new Main
                    {

                    };
                    this.Close();
                    ShowMain.Show();
                }
                else
                {

                }
            }

        }

        private void Btn_Setting_LoginPage_Click(object sender, RoutedEventArgs e)
        {
            SettingsDB ShowSettingsDBWinddow = new SettingsDB
            {

            };
            ShowSettingsDBWinddow.ShowDialog();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
