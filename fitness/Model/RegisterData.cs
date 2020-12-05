using Microsoft.Win32;
using System;
using System.Data.Entity.Core.EntityClient;
using System.Windows;

namespace fitness.Model
{
    class RegisterData
    {
        public void SaveUserNameToRegistry(string userName)
        {
            RegistryKey usrNameKey = Registry.CurrentUser.CreateSubKey("Software\\Fitness");
            usrNameKey.SetValue("UserNameRegister", userName); // User Name Save To Registry Varible 
            usrNameKey.Close();
        }
        public string LoadDataOnRegistry()
        {
            string userName = "";
            RegistryKey getUsrKey = Registry.CurrentUser.CreateSubKey("Software\\Fitness");
            userName = (string)getUsrKey.GetValue("UserNameRegister");
            return userName;
        }
        public void SaveConnectionStringOnRegistry(string entityConnectionString, string ServerName, string DBName, string Username, string Password)
        {
            RegistryKey ConnectionString = Registry.CurrentUser.CreateSubKey("Software\\Fitness"); // path Registry
            try
            {
                // ConnectionString.SetValue("ConncetionString", entityConnectionString); // ConncetionString Save To Registry Varible  // cryptionAlgorithm.EncryptTextUsingUTF8 this method For encryption ConnectionString
                ConnectionString.SetValue("ConncetionString", cryptionAlgorithm.EncryptTextUsingUTF8(entityConnectionString)); // ConncetionString Save To Registry Varible  // cryptionAlgorithm.EncryptTextUsingUTF8 this method For encryption ConnectionString
                ///Save encriptServername For CrystalReport
                ConnectionString.SetValue("srLog", cryptionAlgorithm.EncryptTextUsingUTF8(ServerName)); // ConncetionString Save To Registry Varible  // cryptionAlgorithm.EncryptTextUsingUTF8 this method For encryption ConnectionString
                ///Save encriptDBname For CrystalReport
                ConnectionString.SetValue("dblog", cryptionAlgorithm.EncryptTextUsingUTF8(DBName)); // ConncetionString Save To Registry Varible  // cryptionAlgorithm.EncryptTextUsingUTF8 this method For encryption ConnectionString
                ///Save encriptPassword For CrystalReport
                ConnectionString.SetValue("pblog", cryptionAlgorithm.EncryptTextUsingUTF8(Password)); // ConncetionString Save To Registry Varible  // cryptionAlgorithm.EncryptTextUsingUTF8 this method For encryption ConnectionString

                ///Save encripUserName For CrystalReport
                ConnectionString.SetValue("ulog", cryptionAlgorithm.EncryptTextUsingUTF8(Username)); // ConncetionString Save To Registry Varible  // cryptionAlgorithm.EncryptTextUsingUTF8 this method For encryption ConnectionString

            }
            catch (Exception exTwo)
            {
                //db.SP_Insert_ErrorLogs(UserInfo.UserId, exTwo.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "ConncectToDBWindow Page  Btn_setSettingsDB_Click", UserInfo.Name + " " + UserInfo.Family, infoSystems.ComputerName, infoSystems.IpAddress);

                MessageBox.Show("Registery Error : \n\n\n" + exTwo.ToString());
            }
            finally
            {
                ConnectionString.Close();
            }
        }
        //Create Connection String
        public string BuildEntityConnection(string ADOconnectionstring)
        {
            var entityConection = new EntityConnectionStringBuilder
            {
                Provider = "System.Data.SqlClient",
                ProviderConnectionString = ADOconnectionstring,
                Metadata = "res://*"
            };
            return entityConection.ToString();
        } // Create Dynamic ConnectionString
        public bool LoadConnectionString(string getConnectionString) // Get ConnectionString from Registy
        {
            //string getConnectionString = "";
            RegistryKey ConnectionString = Registry.CurrentUser.CreateSubKey("Software\\Fitness"); // path for check existing ConnectionString
            getConnectionString = (string)ConnectionString.GetValue("ConncetionString");
            try
            {
                if (getConnectionString == "" || getConnectionString == null)
                {

                    return false;

                }

                //Decrypt ConnectionString

                dbConnection.ConnectionString = cryptionAlgorithm.DecryptTextUsingUTF8(getConnectionString);


            }
            catch (Exception ex)
            {
                //btn_Login.IsEnabled = false;

                MessageBox.Show("Error : Connection String not exist : \n\n\n " + ex.ToString());
            }
            return true;



        }

    }
}
