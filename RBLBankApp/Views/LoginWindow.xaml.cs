// ===============================================
// FILE: Views/LoginWindow.xaml.cs
// ===============================================

using System;
using System.Data.SqlClient;
using System.Windows;
using RBLBankApp.Services;

namespace RBLBankApp.Views
{
    public partial class LoginWindow : Window
    {
        DatabaseService db = new DatabaseService();

        public LoginWindow()
        {
            InitializeComponent();
        }

        // =========================================================
        // LOGIN BUTTON
        // =========================================================
        // Definition:
        // Validates user login credentials.
        //
        // Approach:
        // 1. Read username/password
        // 2. Check database
        // 3. Open dashboard if valid
        //
        // Time Complexity:
        // O(1)
        // =========================================================

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            txtError.Text = "";

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (username == "" || password == "")
            {
                txtError.Text = "All fields are required!";
                return;
            }

            try
            {
                string query =
                    "SELECT Role FROM Users WHERE Username=@u AND Password=@p";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@u", username),
                    new SqlParameter("@p", password)
                };

                var dt = db.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    DashboardWindow dashboard = new DashboardWindow();

                    dashboard.UserRole = dt.Rows[0]["Role"].ToString();

                    dashboard.Show();

                    this.Close();
                }
                else
                {
                    txtError.Text = "Invalid Username or Password!";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // =========================================================
        // NAVIGATE TO REGISTER
        // =========================================================

        private void GoToRegister_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow register = new RegisterWindow();

            register.Show();

            this.Close();
        }
    }
}