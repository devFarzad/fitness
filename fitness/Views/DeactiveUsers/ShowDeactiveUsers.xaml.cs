using DataModelFitness;
using fitness.Model;
using System;
using System.Linq;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;

namespace fitness.Views.DeactiveUsers
{
    /// <summary>
    /// Interaction logic for ShowDeactiveUsers.xaml
    /// </summary>
    public partial class ShowDeactiveUsers : Window
    {
        public ShowDeactiveUsers()
        {
            InitializeComponent();
            getCurrentDate();
        }

        private void LoadMembersData(Func<string> FuncGenerate)
        {
            try
            {
                decimal TotalAmount = 0;
                DataGrid_Members.ItemsSource = null;
                using (fitnessEntities db = new fitnessEntities(dbConnection.ConnectionString))
                {
                    var queryLoadMembers = db.Database.SqlQuery<VW_members>("select * From VW_members where  users_Status=0 " + FuncGenerate() + " ORDER BY usersInfoFitness_StartDate DESC");
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

                    }
                    else
                    {
                        DataGrid_Members.ItemsSource = null;

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : \n\n\n " + ex.ToString());
            }

        }
        private string GenerateSearchKey()
        {


            string searchKey = "";
            if (!string.IsNullOrEmpty(date_FromStart.Text) && !string.IsNullOrEmpty(date_ToStart.Text))
            {

                searchKey = "and usersInfoFitness_StartDate Between '" + String.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(date_FromStart.Text)) + " " + "00:00:00" + "' And '" + String.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(date_ToStart.Text)) + " " + "23:59:59" + "'";
            }
            if (!string.IsNullOrEmpty(txt_SearchSubscribeFullName.Text))
            {
                searchKey += " and FullName Like N'%" + txt_SearchSubscribeFullName.Text.Trim() + "%'";
            }


            return searchKey;

        }


        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

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


        private void btn_AactiveMember_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                object item = DataGrid_Members.SelectedItem;

                if (item != null)
                {
                    int UserId = Convert.ToInt32((DataGrid_Members.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text);
                    string UserFullName = (DataGrid_Members.SelectedCells[1].Column.GetCellContent(item) as TextBlock).Text;
                    if (!string.IsNullOrEmpty(UserFullName))
                    {
                        if (MessageBox.Show("Are you sure ? ", "Active", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            using (fitnessEntities db = new fitnessEntities(dbConnection.ConnectionString))
                            {

                                db.disableUserMember(UserId, 1); //0 for Desible 1 for Active 
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

                LoadMembersData(GenerateSearchKey);

            }

        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                object item = DataGrid_Members.SelectedItem;

                if (item != null)
                {
                    int UserId = Convert.ToInt32((DataGrid_Members.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text);
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
                                    var querySelectAuthUser = db.Database.SqlQuery<vw_AuthUsers>("select * From vw_AuthUsers where PK_users_Id=" + UserId + " and FK_user_Auth_role_Id=1");
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

                //Reload Data
                LoadMembersData(GenerateSearchKey);

            }

        }

        private void Btn_Search_Click(object sender, RoutedEventArgs e)
        {
            LoadMembersData(GenerateSearchKey);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadMembersData(GenerateSearchKey);

        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void PanelDeactiveList_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void pnl_Search_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
