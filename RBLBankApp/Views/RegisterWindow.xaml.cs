// =========================================================
// FILE: Views/RegisterWindow.xaml.cs
// =========================================================

using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using RBLBankApp.Services;

namespace RBLBankApp.Views
{
    public partial class RegisterWindow : Window
    {
        // =====================================================
        // DATABASE OBJECT
        // =====================================================

        DatabaseService db = new DatabaseService();

        // =====================================================
        // PASSWORD VISIBILITY VARIABLE
        // =====================================================

        private bool isPasswordVisible = false;

        // =====================================================
        // CONSTRUCTOR
        // =====================================================

        public RegisterWindow()
        {
            InitializeComponent();
        }

        // =====================================================
        // TOGGLE PASSWORD
        // =====================================================
        // Definition:
        // Shows or hides password.
        //
        // Time Complexity:
        // O(1)
        // =====================================================

        private void TogglePassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Password visibility feature can be extended using TextBox switch.",
                "Info",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        // =====================================================
        // REGISTER BUTTON
        // =====================================================
        // Definition:
        // Registers new user into database.
        //
        // Approach:
        // 1. Validate fields
        // 2. Check duplicate username
        // 3. Insert user
        //
        // Time Complexity:
        // O(1)
        // =====================================================

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            txtError.Text = "";

            string fullName = txtFullName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password.Trim();

            // VALIDATION

            if (fullName == "" ||
                username == "" ||
                email == "" ||
                password == "")
            {
                txtError.Text = "All fields are required!";
                return;
            }

            // EMAIL VALIDATION

            if (!Regex.IsMatch(
                email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                txtError.Text = "Invalid email address!";
                return;
            }

            // PASSWORD VALIDATION

            if (password.Length < 4)
            {
                txtError.Text =
                    "Password must be at least 4 characters!";
                return;
            }

            try
            {
                // =============================================
                // CHECK DUPLICATE USERNAME
                // =============================================

                string checkQuery =
                    "SELECT COUNT(*) FROM Users WHERE Username=@u";

                int count = Convert.ToInt32(
                    db.ExecuteScalar(
                        checkQuery,
                        new SqlParameter[]
                        {
                            new SqlParameter("@u", username)
                        }));

                if (count > 0)
                {
                    txtError.Text =
                        "Username already exists!";
                    return;
                }

                // =============================================
                // INSERT USER
                // =============================================

                string insertQuery =
                    @"INSERT INTO Users
                    (
                        FullName,
                        Username,
                        Email,
                        Password,
                        Role
                    )
                    VALUES
                    (
                        @f,
                        @u,
                        @e,
                        @p,
                        @r
                    )";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@f", fullName),
                    new SqlParameter("@u", username),
                    new SqlParameter("@e", email),
                    new SqlParameter("@p", password),
                    new SqlParameter("@r", "User")
                };

                int rows =
                    db.ExecuteNonQuery(insertQuery, parameters);

                // =============================================
                // SUCCESS
                // =============================================

                if (rows > 0)
                {
                    MessageBox.Show(
                        "Registration Successful!",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    LoginWindow login = new LoginWindow();

                    login.Show();

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // =====================================================
        // LOGIN LINK
        // =====================================================

        private void GoToLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();

            login.Show();

            this.Close();
        }
    }
}