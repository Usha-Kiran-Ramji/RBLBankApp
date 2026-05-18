using RBLBankApp.Services;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;

namespace RBLBankApp.Views
{
    public partial class DashboardWindow : Window
    {
        private DatabaseService db = new DatabaseService();

        // ✅ FIXED USER ROLE PROPERTY
        public string UserRole { get; set; }

        public DashboardWindow()
        {
            InitializeComponent();
            ShowSection("HOME");
        }

        // ================= UI SECTION SWITCH =================
        private void ShowSection(string section)
        {
            HomeSection.Visibility = Visibility.Collapsed;
            UsersSection.Visibility = Visibility.Collapsed;
            AccountsSection.Visibility = Visibility.Collapsed;
            TransactionsSection.Visibility = Visibility.Collapsed;

            if (section == "HOME")
            {
                HomeSection.Visibility = Visibility.Visible;
                LoadDashboardStats();
            }
            else if (section == "USERS")
            {
                UsersSection.Visibility = Visibility.Visible;
                LoadUsers();
            }
            else if (section == "ACCOUNTS")
            {
                AccountsSection.Visibility = Visibility.Visible;
                LoadAccounts();
            }
            else if (section == "TRANSACTIONS")
            {
                TransactionsSection.Visibility = Visibility.Visible;
                LoadTransactionForm();
            }
        }

        // ================= NAVIGATION =================
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            ShowSection("HOME");
        }

        private void Users_Click(object sender, RoutedEventArgs e)
        {
            ShowSection("USERS");
        }

        private void Accounts_Click(object sender, RoutedEventArgs e)
        {
            ShowSection("ACCOUNTS");
        }

        private void Transactions_Click(object sender, RoutedEventArgs e)
        {
            ShowSection("TRANSACTIONS");
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();

            this.Close();
        }

        // ================= DASHBOARD =================
        private void LoadDashboardStats()
        {
            try
            {
                int users = Convert.ToInt32(
                    db.ExecuteScalar("SELECT COUNT(*) FROM Users") ?? 0);

                int accounts = Convert.ToInt32(
                    db.ExecuteScalar("SELECT COUNT(*) FROM Accounts") ?? 0);

                decimal balance = Convert.ToDecimal(
                    db.ExecuteScalar("SELECT ISNULL(SUM(Balance),0) FROM Accounts") ?? 0);

                int transactions = Convert.ToInt32(
                    db.ExecuteScalar("SELECT COUNT(*) FROM Transactions") ?? 0);

                txtTotalUsers.Text = users.ToString();
                txtTotalAccounts.Text = accounts.ToString();
                txtTotalBalance.Text = "₹ " + balance.ToString("N2");
                txtTotalTransactions.Text = transactions.ToString();

                // BAR CHART
                BarChart.Series = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Data",
                        Values = new ChartValues<int>
                        {
                            users,
                            accounts
                        }
                    }
                };

                BarChart.AxisX.Clear();

                BarChart.AxisX.Add(new Axis
                {
                    Labels = new[] { "Users", "Accounts" }
                });

                // PIE CHART
                PieChart.Series = new SeriesCollection
                {
                    new PieSeries
                    {
                        Title = "Balance",
                        Values = new ChartValues<decimal>
                        {
                            balance
                        },
                        DataLabels = true
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dashboard Error: " + ex.Message);
            }
        }

        // ================= USERS =================
        private void LoadUsers()
        {
            try
            {
                var view = db.ExecuteQuery("SELECT * FROM Users").DefaultView;
                UsersGrid.ItemsSource = view;
                txtEmptyUsers.Visibility = view.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SearchUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string key = txtSearchUser.Text.Trim();

                if (string.IsNullOrEmpty(key))
                {
                    LoadUsers();
                    return;
                }

                string query =
                    "SELECT * FROM Users WHERE FullName LIKE @k OR Username LIKE @k";

                var view = db.ExecuteQuery(
                        query,
                        new SqlParameter[]
                        {
                            new SqlParameter("@k", "%" + key + "%")
                        }).DefaultView;
                
                UsersGrid.ItemsSource = view;
                txtEmptyUsers.Visibility = view.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            UserForm form = new UserForm();
            form.ShowDialog();

            LoadUsers();
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem == null)
            {
                MessageBox.Show("Select user!");
                return;
            }

            DataRowView row = (DataRowView)UsersGrid.SelectedItem;

            UserForm form = new UserForm();

            form.UserID = Convert.ToInt32(row["UserID"]);

            form.txtFullName.Text = row["FullName"].ToString();
            form.txtUsername.Text = row["Username"].ToString();
            form.txtEmail.Text = row["Email"].ToString();

            form.ShowDialog();

            LoadUsers();
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem == null)
            {
                MessageBox.Show("Select user!");
                return;
            }

            DataRowView row = (DataRowView)UsersGrid.SelectedItem;

            db.ExecuteNonQuery(
                "DELETE FROM Users WHERE UserID=@id",
                new SqlParameter[]
                {
                    new SqlParameter("@id",
                    Convert.ToInt32(row["UserID"]))
                });

            LoadUsers();
        }

        private void UsersGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Password" || e.PropertyName == "UserID")
            {
                e.Cancel = true;
            }
             
            // Optional styling for headers
            if (e.Column != null)
            {
                e.Column.Header = e.PropertyName; // Simplistic header naming
            }
        }

        // ================= ACCOUNTS =================
        private void LoadAccounts()
        {
            try
            {
                var view = db.ExecuteQuery("SELECT * FROM Accounts").DefaultView;
                AccountsGrid.ItemsSource = view;
                txtEmptyAccounts.Visibility = view.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SearchAccount_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string key = txtSearchAccount.Text.Trim();

                var view = db.ExecuteQuery(
                        "SELECT * FROM Accounts WHERE AccountNumber LIKE @k",
                        new SqlParameter[]
                        {
                            new SqlParameter("@k", "%" + key + "%")
                        }).DefaultView;

                AccountsGrid.ItemsSource = view;
                txtEmptyAccounts.Visibility = view.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddAccount_Click(object sender, RoutedEventArgs e)
        {
            AccountForm form = new AccountForm();

            form.ShowDialog();

            LoadAccounts();
        }

        // ================= TRANSACTIONS =================
        private void LoadTransactionForm()
        {
            try
            {
                var view = db.ExecuteQuery("SELECT * FROM Transactions").DefaultView;
                TransactionsGrid.ItemsSource = view;
                txtEmptyTransactions.Visibility = view.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Transfer_Click(object sender, RoutedEventArgs e)
        {
            // ✅ FIXED ERROR
            TransactionForm form = new TransactionForm();

            form.ShowDialog();

            LoadTransactionForm();
        }
    }
}