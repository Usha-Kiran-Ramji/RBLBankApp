using System.Data.SqlClient;
using System.Windows;
using RBLBankApp.Services;

namespace RBLBankApp.Views
{
    public partial class AccountForm : Window
    {
        DatabaseService db = new DatabaseService();

        public AccountForm()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string query = @"INSERT INTO Accounts 
                            (UserID, AccountNumber, Balance)
                            VALUES (@uid, @acc, @bal)";

            SqlParameter[] parameters =
            {
                new SqlParameter("@uid", int.Parse(txtUserID.Text)),
                new SqlParameter("@acc", txtAccountNumber.Text),
                new SqlParameter("@bal", decimal.Parse(txtBalance.Text))
            };

            db.ExecuteNonQuery(query, parameters);

            MessageBox.Show("Account Created!");
            this.Close();
        }
    }
}
