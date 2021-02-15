using fitness.Model;
using System;
using System.Windows;
using System.Windows.Input;

namespace fitness.Views.ManageConnectionString
{
    /// <summary>
    /// Interaction logic for SettingsDB.xaml
    /// </summary>
    public partial class SettingsDB : Window
    {
        public SettingsDB()
        {
            InitializeComponent();
            DirectionLanguages();
        }
        RegisterData registry = new RegisterData();
        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void DirectionLanguages()
        {
            switch (Properties.Settings.Default.languageCode)
            {
                case "ku-Arab-IQ":
                    Pnl_DBSetting.FlowDirection = FlowDirection.RightToLeft;
                    break;
                case "ar-IQ":
                    Pnl_DBSetting.FlowDirection = FlowDirection.RightToLeft;
                    break;
                case "fa-IR":
                    Pnl_DBSetting.FlowDirection = FlowDirection.RightToLeft;
                    break;
                case "en-US":
                    Pnl_DBSetting.FlowDirection = FlowDirection.LeftToRight;
                    break;
            }
        }
        private bool checkNullable()
        {
            if (string.IsNullOrEmpty(txt_DBName.Text.Trim()))
            {
                MessageBox.Show("Please Enter Database Name ");
                return false;
            }
            else if (string.IsNullOrEmpty(txt_ServerName.Text.Trim()))
            {
                MessageBox.Show("Please Enter ServerName  ");
                return false;
            }
            else if (string.IsNullOrEmpty(txt_UserName.Text.Trim()))
            {
                MessageBox.Show("Please Enter UserName  ");
                return false;
            }
            else if (string.IsNullOrEmpty(pw_Password.Password.Trim()))
            {
                MessageBox.Show("Please Enter Password  ");
                return false;
            }
            return true;
        }
        private void Btn_Save_DBSetting_Click(object sender, RoutedEventArgs e)
        {
            if (!checkNullable())
            {
                return;
            }
            else
            {
                //Create daynamic ConnectionString
                var entityConnectionString = registry.BuildEntityConnection("Data Source=" +
                  txt_ServerName.Text.Trim() +
                  "; Initial Catalog=" +
                  txt_DBName.Text.Trim() +
                  "; user id=" +
                  txt_UserName.Text.Trim() +
                  "; Password=" +
                  pw_Password.Password +
                  "; Integrated Security=false");// for convert ConnectionString Ado to EntityFramwork
                try
                {
                    registry.SaveConnectionStringOnRegistry(entityConnectionString,
                                                            txt_ServerName.Text.Trim(),
                                                            txt_DBName.Text.Trim(),
                                                            txt_UserName.Text.Trim(),
                                                            pw_Password.Password);
                    MessageBox.Show("Connected to server successfully.",
                                    "Set Connection",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);

                    this.Close();
                }
                catch (Exception ex)
                {
                    //  db.SP_Insert_ErrorLogs(UserInfo.UserId, ex.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "ConncectToDBWindow Page  Btn_setSettingsDB_Click", UserInfo.Name + " " + UserInfo.Family, infoSystems.ComputerName, infoSystems.IpAddress);

                    MessageBox.Show("Error : \n\n\n" + ex.ToString());
                }
                finally
                {
                }
            }

        }

        private void PanelDbInfo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Btn_License_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
