using DataModelFitness;
using fitness.Model;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;

namespace fitness.Views.MainPage
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
            getCurrentDate();
        }
        private double getHeight { get; set; }
        private double getWeight { get; set; }
        private double BMIResult { get; set; }
        private double BMRResult { get; set; }
        private byte StatusSearch { get; set; }// 1 for normal Search 2 for ExpireMembers
        private byte btnAddOrUpdate { get; set; }//1=>Add , 2->update 
        private int UserId { get; set; }
        private int CountExpiredDate { get; set; }
        private void btn_Icon_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void getCurrentDate()
        {
            //Get Current Date
            DateTime startDate = Convert.ToDateTime(DateTime.Now.ToString());

            // Add First day of current month to DataPicker FromDate
            var firstDayOfMonth = new DateTime(startDate.Year, startDate.Month, 1);

            // Add Last day of current month to DataPicker FromDate
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            date_FromStart.Text = firstDayOfMonth.ToString();
            date_ToStart.Text = lastDayOfMonth.ToString();
        }
        private void Btn_New_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!checkNullable())
                {
                    return;
                }
                else
                {
                    if (this.btnAddOrUpdate == 1) // For New 
                    {
                        //todo add new member with query
                        using (fitnessEntities db = new fitnessEntities(dbConnection.ConnectionString))
                        {
                            using (TransactionScope ts = new TransactionScope())
                            {

                                var querySelectMember = db.Database.SqlQuery<VW_members>("select * From VW_members where  FullName =N'" + txt_ShowFirstName.Text.Trim() + ' ' + txt_ShowLastName.Text.Trim() + "'");
                                var resultSelectMember = querySelectMember.ToList();
                                if (resultSelectMember.Count == 1)
                                {
                                    MessageBox.Show("User Exist ! ");
                                    return;
                                }
                                else
                                {
                                    //Save Members info Part 1
                                    db.addMember(
                                        txt_ShowFirstName.Text.Trim(),
                                        txt_ShowLastName.Text.Trim(),
                                        txt_phoneNumber.Text.Trim(),
                                        txt_address.Text.Trim(),
                                      String.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) +
                                      ' ' +
                                      String.Format("{0:HH:mm:ss}", Convert.ToDateTime(DateTime.Now.ToString("hh:mm:ss"))),
                                      1,
                                      txt_Description.Text.Trim()
                                        );
                                    db.SaveChanges();
                                    //select Id New Member

                                    var querySelectMemberId = db.Database.SqlQuery<User>("select * From Users where  users_FirstName Like N'%" + txt_ShowFirstName.Text.Trim() + "%' and users_LastName Like N'%" + txt_ShowLastName.Text.Trim() + "%'");
                                    var resultSelectMemberId = querySelectMemberId.ToList();

                                    if (resultSelectMemberId.Count == 0)
                                    {
                                        MessageBox.Show("Transaction Canceled !");
                                        return;
                                    }
                                    //Save Members info Part 2
                                    db.AddmemberInfo(resultSelectMemberId[0].PK_users_Id,
                                        Convert.ToDouble(txt_Height.Text.Trim()),
                                        Convert.ToDouble(txt_Weight.Text.Trim()),
                                        Convert.ToDouble(txt_waistMeasure.Text.Trim()),
                                        String.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(date_Start.Text)) +
                                   ' ' +
                                   String.Format("{0:HH:mm:ss}", Convert.ToDateTime(DateTime.Now.ToLongTimeString())),
                                        String.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(date_End.Text)) +
                                   ' ' +
                                  String.Format("{0:HH:mm:ss}", Convert.ToDateTime(DateTime.Now.ToLongTimeString())),
                                        "",
                                        Convert.ToInt32(txt_Age.Text.Trim()),
                                        Convert.ToDecimal(txt_MonthlyAmount.Text.Trim()),
                                       Convert.ToDouble(this.BMRResult),
                                       Convert.ToDouble(this.BMIResult),

                                        String.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) +
                                      ' ' +
                                      String.Format("{0:HH:mm:ss}", Convert.ToDateTime(DateTime.Now.ToString("hh:mm:ss"))),
                                        cmb_exerciesLevel.SelectedIndex,
                                        cmb_exerciesLevel.Text
                                        );
                                    db.SaveChanges();



                                    MessageBox.Show("Save Successed !");
                                }
                                ClearData();
                                ts.Complete();
                            }
                        }

                    }
                    else if (this.btnAddOrUpdate == 2) // For update 
                    {
                        //todo update member with query
                        if (this.UserId > 0) //Select Member
                        {
                            using (fitnessEntities db = new fitnessEntities(dbConnection.ConnectionString))
                            {
                                using (TransactionScope ts = new TransactionScope())
                                {
                                    db.updateMember(
                                        this.UserId, txt_ShowFirstName.Text.Trim(), txt_ShowLastName.Text.Trim(), txt_phoneNumber.Text.Trim(), txt_address.Text.Trim(), 1, txt_Description.Text
                                        );
                                    db.SaveChanges();


                                    db.updateMemberInfo(
                                        this.UserId,
                                        Convert.ToDouble(txt_Height.Text.Trim()),
                                        Convert.ToDouble(txt_Weight.Text.Trim()),
                                        Convert.ToDouble(txt_waistMeasure.Text.Trim()),
                                          String.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(date_Start.Text)) +
                                   ' ' +
                                   String.Format("{0:HH:mm:ss}", Convert.ToDateTime(DateTime.Now.ToLongTimeString())),

                                        String.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(date_End.Text)) +
                                   ' ' +
                                  String.Format("{0:HH:mm:ss}", Convert.ToDateTime(DateTime.Now.ToLongTimeString())),
                                        "", Convert.ToInt32(txt_Age.Text.Trim()),
                                        Convert.ToDecimal(txt_MonthlyAmount.Text.Trim()),
                                       this.BMRResult,
                                       this.BMIResult,
                                        cmb_exerciesLevel.SelectedIndex,
                                        cmb_exerciesLevel.Text
                                       );
                                    db.SaveChanges();

                                    MessageBox.Show("Update Success !");
                                    this.btnAddOrUpdate = 1;
                                    Btn_New.Content = "New";
                                    ClearData();
                                    ts.Complete();
                                }
                            }
                        }



                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : \n\n\n " + ex.ToString());
            }
            finally
            {

                LoadMembersData(GenerateSearchKey);
                LoadExpireMembers();
            }
        }
        private void ClearData()
        {
            txt_address.Text = "";
            txt_Age.Text = "";
            txt_Description.Text = "";
            txt_Height.Text = "";
            txt_MonthlyAmount.Text = "0";
            txt_Age.Text = "";
            txt_ShowFirstName.Text = "";
            txt_ShowLastName.Text = "";
            txt_waistMeasure.Text = "";
            txt_Weight.Text = "";
            txt_phoneNumber.Text = "";
            date_End.Text = "";
            date_Start.Text = "";

        }
        private void LoadMembersData(Func<string> FuncGenerate)
        {
            try
            {
                decimal TotalAmount = 0;
                DataGrid_Members.ItemsSource = null;
                using (fitnessEntities db = new fitnessEntities(dbConnection.ConnectionString))
                {
                    var queryLoadMembers = db.Database.SqlQuery<VW_members>("select * From VW_members where  users_Status=1 " + FuncGenerate() + " ORDER BY usersInfoFitness_StartDate DESC");
                    var resultLoadMembers = queryLoadMembers.ToList();
                    if (resultLoadMembers.Count > 0)
                    {
                        for (int i = 0; i < resultLoadMembers.Count; i++)
                        {
                            //this.CountExpiredDate += ExpiredDate(resultLoadMembers[i].usersInfoFitness_EndDate,
                            //                                               String.Format("{0:yyyy-MM-dd}", DateTime.Now));
                            TotalAmount += Convert.ToDecimal(resultLoadMembers[i].usersInfoFitness_MonthlyAmount);
                        }
                        DataGrid_Members.ItemsSource = resultLoadMembers;
                        txt_Total_MontlyAmount.Text = string.Format(new System.Globalization.CultureInfo("ar-IQ"),
                                                                                "{0:C0}",
                                                                                TotalAmount);
                    }
                    else
                    {
                        DataGrid_Members.ItemsSource = null;
                        txt_Total_MontlyAmount.Text = string.Format(new System.Globalization.CultureInfo("ar-IQ"),
                                                                               "{0:C0}",
                                                                               0);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : \n\n\n " + ex.ToString());
            }

        }
        private void LoadExpireMembers()
        {
            try
            {
                this.CountExpiredDate = 0;
                using (fitnessEntities db = new fitnessEntities(dbConnection.ConnectionString))
                {
                    var queryLoadMembers = db.Database.SqlQuery<VW_members>("select * From VW_members where users_Status=1");
                    var resultLoadMembers = queryLoadMembers.ToList();
                    if (resultLoadMembers.Count > 0)
                    {
                        for (int i = 0; i < resultLoadMembers.Count; i++)
                        {
                            this.CountExpiredDate += ExpiredDate(resultLoadMembers[i].usersInfoFitness_EndDate,
                                                                           String.Format("{0:yyyy-MM-dd}", DateTime.Now));
                        }

                        txt_ExpiredAccounts.Text = Convert.ToString(this.CountExpiredDate);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : \n\n\n " + ex.ToString());
            }

        }
        private int ExpiredDate(string ExpireDate, string CurrentDate)
        {
            int counter = 0;

            if (DateTime.Parse(ExpireDate) <= DateTime.Parse(CurrentDate))
            {
                return ++counter;
            }

            return counter;
        }

        private string GenerateSearchKey()
        {


            string searchKey = "";
            switch (this.StatusSearch)
            {
                case 1:

                    if (!string.IsNullOrEmpty(date_FromStart.Text) && !string.IsNullOrEmpty(date_ToStart.Text))
                    {
                        //string dateFromString = date_FromStart + " " + "00:00:00";
                        //string dateToString = date_ToStart + " " + "23:59:59";
                        searchKey = "and usersInfoFitness_StartDate Between '" + String.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(date_FromStart.Text)) + " " + "00:00:00" + "' And '" + String.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(date_ToStart.Text)) + " " + "23:59:59" + "'";
                    }
                    if (!string.IsNullOrEmpty(txt_SearchSubscribeFullName.Text))
                    {
                        searchKey += " and FullName Like N'%" + txt_SearchSubscribeFullName.Text.Trim() + "%'";
                    }
                    break;

                case 2:

                    searchKey = " and usersInfoFitness_EndDate <='" +
                        String.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(DateTime.Now.Date)) +
                        "'";
                    break;

            }


            return searchKey;

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClearData();
            this.btnAddOrUpdate = 1;
            Btn_New.Content = "New";
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                object item = DataGrid_Members.SelectedItem;
                this.UserId = 0;
                if (item != null)
                {
                    this.UserId = Convert.ToInt32((DataGrid_Members.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text);
                    string UserFullName = (DataGrid_Members.SelectedCells[1].Column.GetCellContent(item) as TextBlock).Text;
                    if (!string.IsNullOrEmpty(UserFullName))
                    {
                        if (MessageBox.Show("Are you sure ? ", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            //todo Delete member with Query
                            using (fitnessEntities db = new fitnessEntities(dbConnection.ConnectionString))
                            {
                                using (TransactionScope ts = new TransactionScope())
                                {
                                    var querySelectAuthUser = db.Database.SqlQuery<vw_AuthUsers>("select * From vw_AuthUsers where PK_users_Id=" + this.UserId + " and FK_user_Auth_role_Id=1");
                                    var resultSelectAuthUser = querySelectAuthUser.ToList();
                                    if (resultSelectAuthUser.Count == 1)
                                    {
                                        MessageBox.Show("You can not delete this user, this user is the system admin !");
                                        return;
                                    }
                                    else
                                    {
                                        db.DeleteFromMemberInfo(UserId);
                                        db.DeleteFromUsers(UserId);
                                        db.SaveChanges();
                                    }

                                    ts.Complete();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : \n\n\n " + ex.ToString());
            }
            finally
            {
                this.StatusSearch = 1; //Normal Search
                //Reload Data
                LoadMembersData(GenerateSearchKey);
                LoadExpireMembers();
            }
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txt_phoneNumber_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex rExpersion = new Regex("[^0-9]+"); // just number input //
            e.Handled = rExpersion.IsMatch(e.Text);
        }

        private void btn_Edit_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                object item = DataGrid_Members.SelectedItem;
                this.UserId = 0;
                if (item != null)
                {
                    this.UserId = Convert.ToInt32((DataGrid_Members.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text);
                    string UserFullName = (DataGrid_Members.SelectedCells[1].Column.GetCellContent(item) as TextBlock).Text;
                    if (!string.IsNullOrEmpty(UserFullName))
                    {
                        if (MessageBox.Show("Are you sure ? ", "Edit", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            using (fitnessEntities db = new fitnessEntities(dbConnection.ConnectionString))
                            {
                                var querySelectMember = db.Database.SqlQuery<VW_members>("select * From VW_members where PK_users_Id=" + this.UserId);
                                var resultSelectMember = querySelectMember.ToList();
                                if (resultSelectMember.Count == 1)
                                {
                                    txt_ShowFirstName.Text = resultSelectMember[0].users_FirstName;
                                    txt_ShowLastName.Text = resultSelectMember[0].users_LastName;
                                    txt_phoneNumber.Text = resultSelectMember[0].users_PhoneNumber;
                                    txt_address.Text = resultSelectMember[0].users_Address;
                                    date_Start.Text = resultSelectMember[0].usersInfoFitness_StartDate;
                                    date_End.Text = resultSelectMember[0].usersInfoFitness_EndDate;
                                    txt_Age.Text = Convert.ToString(resultSelectMember[0].usersInfoFitness_Age);
                                    txt_Height.Text = Convert.ToString(resultSelectMember[0].usersInfoFitness_Height);
                                    txt_Weight.Text = Convert.ToString(resultSelectMember[0].usersInfoFitness_Weight);
                                    txt_waistMeasure.Text = Convert.ToString(resultSelectMember[0].usersInfoFitness_WaistMeasure);
                                    txt_MonthlyAmount.Text = Convert.ToString(resultSelectMember[0].usersInfoFitness_MonthlyAmount);
                                    txt_Description.Text = resultSelectMember[0].users_Description;
                                    Btn_New.Content = "Update";
                                    cmb_exerciesLevel.SelectedIndex = Convert.ToInt32(resultSelectMember[0].usersInfoFitness_exerciesLevel);
                                    this.btnAddOrUpdate = 2;//For Update Btn //Change Action btn_new for update

                                }
                                else
                                {
                                    MessageBox.Show("There is more than one member in the system !");
                                    return;
                                }


                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : \n\n\n + " + ex.ToString());
            }
            finally
            {
                this.StatusSearch = 1; //Normal Search
                LoadMembersData(GenerateSearchKey);
                LoadExpireMembers();
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            this.btnAddOrUpdate = 1;//For New Btn //Change Action btn_new for New
            this.StatusSearch = 1;//Normal Search
            LoadMembersData(GenerateSearchKey);
            LoadExpireMembers();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void txt_MonthlyAmount_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex rExpersion = new Regex("[^0-9]+"); // just number input //
            e.Handled = rExpersion.IsMatch(e.Text);
        }

        private void txt_Age_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex rExpersion = new Regex("[^0-9]+"); // just number input //
            e.Handled = rExpersion.IsMatch(e.Text);
        }
        private bool checkNullable()
        {
            if (string.IsNullOrEmpty(txt_ShowFirstName.Text.Trim()))
            {
                MessageBox.Show("Please Enter First Name !");
                return false;
            }
            else if (string.IsNullOrEmpty(txt_ShowLastName.Text.Trim()))
            {
                MessageBox.Show("Please Enter First Name !");
                return false;
            }
            else if (string.IsNullOrEmpty(txt_phoneNumber.Text.Trim()))
            {
                MessageBox.Show("Please Enter First Name !");
                return false;
            }
            else if (string.IsNullOrEmpty(txt_address.Text.Trim()))
            {
                MessageBox.Show("Please Enter First Name !");
                return false;
            }
            else if (string.IsNullOrEmpty(txt_Age.Text.Trim()))
            {
                MessageBox.Show("Please Enter Age !");
                return false;
            }
            else if (string.IsNullOrEmpty(txt_Height.Text.Trim()))
            {
                MessageBox.Show("Please Enter Height  !");
                return false;
            }
            else if (string.IsNullOrEmpty(txt_Weight.Text.Trim()))
            {
                MessageBox.Show("Please Enter Weight  !");
                return false;
            }
            else if (string.IsNullOrEmpty(txt_waistMeasure.Text.Trim()))
            {
                MessageBox.Show("Please Enter waistMeasur  !");
                return false;
            }
            else if (string.IsNullOrEmpty(txt_MonthlyAmount.Text.Trim()))
            {
                MessageBox.Show("Please Enter MontlyAmount !");
                return false;
            }
            else if (string.IsNullOrEmpty(cmb_exerciesLevel.Text.Trim()))
            {
                MessageBox.Show("Please Select  Exercies Level !");
                return false;
            }
            else if (string.IsNullOrEmpty(date_Start.Text.Trim()))
            {
                MessageBox.Show("Please Select Start Date  !");
                return false;
            }
            else if (string.IsNullOrEmpty(date_End.Text.Trim()))
            {
                MessageBox.Show("Please Select End Date !");
                return false;
            }


            return true;
        }

        private void txt_Height_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_Height.Text.Trim()) && !string.IsNullOrEmpty(txt_Weight.Text.Trim()))
            {
                this.BMIResult = Convert.ToDouble(txt_Weight.Text.Trim()) / ((Convert.ToDouble(txt_Height.Text.Trim()) * Convert.ToDouble(txt_Height.Text.Trim())) / 10.0000);
                txt_BMR.Text = Convert.ToString(this.BMIResult);
            }
            else
            {
                txt_BMR.Text = "";
            }
            if ((!string.IsNullOrEmpty(txt_Height.Text.Trim()) && !string.IsNullOrEmpty(txt_Weight.Text.Trim())) && !string.IsNullOrEmpty(txt_Age.Text.Trim()))
            {
                this.BMRResult = (10 * Convert.ToDouble(txt_Weight.Text.Trim())) + (6.25 * Convert.ToDouble(txt_Height.Text.Trim())) - (5 * Convert.ToDouble(txt_Age.Text.Trim())) + 5;
                txt_BMI.Text = Convert.ToString(this.BMRResult);
            }
            else
            {
                txt_BMI.Text = "";
            }
        }

        private void txt_Age_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_Height.Text.Trim()) && !string.IsNullOrEmpty(txt_Weight.Text.Trim()))
            {
                this.BMIResult = Convert.ToDouble(txt_Weight.Text.Trim()) / ((Convert.ToDouble(txt_Height.Text.Trim()) * Convert.ToDouble(txt_Height.Text.Trim())) / 10.0000);
                txt_BMR.Text = Convert.ToString(this.BMIResult);
            }
            else
            {
                txt_BMR.Text = "";
            }
            if ((!string.IsNullOrEmpty(txt_Height.Text.Trim()) && !string.IsNullOrEmpty(txt_Weight.Text.Trim())) && !string.IsNullOrEmpty(txt_Age.Text.Trim()))
            {
                this.BMRResult = (10 * Convert.ToDouble(txt_Weight.Text.Trim())) + (6.25 * Convert.ToDouble(txt_Height.Text.Trim())) - (5 * Convert.ToDouble(txt_Age.Text.Trim())) + 5;
                txt_BMI.Text = Convert.ToString(this.BMRResult);
            }
            else
            {
                txt_BMI.Text = "";
            }
        }

        private void txt_Weight_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_Height.Text.Trim()) && !string.IsNullOrEmpty(txt_Weight.Text.Trim()))
            {
                this.BMIResult = Convert.ToDouble(txt_Weight.Text.Trim()) / ((Convert.ToDouble(txt_Height.Text.Trim()) * Convert.ToDouble(txt_Height.Text.Trim())) / 10.0000);
                txt_BMR.Text = Convert.ToString(this.BMIResult);
            }
            else
            {
                txt_BMR.Text = "";
            }
            if ((!string.IsNullOrEmpty(txt_Height.Text.Trim()) && !string.IsNullOrEmpty(txt_Weight.Text.Trim())) && !string.IsNullOrEmpty(txt_Age.Text.Trim()))
            {
                this.BMRResult = (10 * Convert.ToDouble(txt_Weight.Text.Trim())) + (6.25 * Convert.ToDouble(txt_Height.Text.Trim())) - (5 * Convert.ToDouble(txt_Age.Text.Trim())) + 5;
                txt_BMI.Text = Convert.ToString(this.BMRResult);
            }
            else
            {
                txt_BMI.Text = "";
            }
        }

        private void Btn_Search_Click(object sender, RoutedEventArgs e)
        {
            this.StatusSearch = 1;
            LoadMembersData(GenerateSearchKey);
            LoadExpireMembers();

        }

        private void TextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.StatusSearch = 2;
            LoadMembersData(GenerateSearchKey);
            LoadExpireMembers();

        }

        private void btn_DeactiveMember_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                object item = DataGrid_Members.SelectedItem;
                this.UserId = 0;
                if (item != null)
                {
                    this.UserId = Convert.ToInt32((DataGrid_Members.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text);
                    string UserFullName = (DataGrid_Members.SelectedCells[1].Column.GetCellContent(item) as TextBlock).Text;
                    if (!string.IsNullOrEmpty(UserFullName))
                    {
                        if (MessageBox.Show("Are you sure ? ", "Disable", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            using (fitnessEntities db = new fitnessEntities(dbConnection.ConnectionString))
                            {

                                db.disableUserMember(this.UserId, 0);
                                db.SaveChanges();



                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : \n\n\n + " + ex.ToString());
            }
            finally
            {
                this.StatusSearch = 1; //Normal Search
                LoadMembersData(GenerateSearchKey);
                LoadExpireMembers();
            }

        }

        private void txt_Height_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }

        private void txt_Weight_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }

        private void txt_waistMeasure_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }
    }
}
