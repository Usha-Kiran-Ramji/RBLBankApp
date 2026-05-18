using RBLBankApp.Services;
using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace RBLBankApp.Views
{
    public partial class AuthWindow : Window
    {
        private DatabaseService db = new DatabaseService();
        private bool isLogin = true;

        public AuthWindow()
        {
            InitializeComponent();
            BuildForms();
            ShowLoginForm();
            
        }

        private void BuildForms()
        {
            // ------------------ LOGIN FORM ------------------
            LoginForm.Children.Clear();

            var loginTitle = new TextBlock
            {
                Text = "Login",
                FontSize = 26,
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.DarkBlue,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var txtUsername = new TextBox { Height = 35, Margin = new Thickness(0, 10, 0, 5), Style = (Style)FindResource("FocusTextBox") };
            txtUsername.ToolTip = "Enter your username";

            var txtPassword = new PasswordBox { Height = 35, Margin = new Thickness(0, 10, 0, 5), Style = (Style)FindResource("FocusPasswordBox") };
            txtPassword.ToolTip = "Enter your password";

            var txtError = new TextBlock { Foreground = System.Windows.Media.Brushes.Red, Margin = new Thickness(0, 10, 0, 0), TextAlignment = TextAlignment.Center };

            var btnLogin = new Button { Content = "Login", Style = (Style)FindResource("PrimaryButton") };
            btnLogin.Click += (s, e) =>
            {
                txtError.Text = "";
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Password.Trim();

                if (string.IsNullOrEmpty(username)) { txtError.Text = "Username required"; return; }
                if (string.IsNullOrEmpty(password)) { txtError.Text = "Password required"; return; }

                try
                {
                    string query = "SELECT * FROM Users WHERE Username=@u AND Password=@p";
                    SqlParameter[] parameters = { new SqlParameter("@u", username), new SqlParameter("@p", password) };
                    var dt = db.ExecuteQuery(query, parameters);

                    if (dt.Rows.Count > 0)
                    {
                        string role = dt.Rows[0]["Role"].ToString();
                        DashboardWindow dashboard = new DashboardWindow { UserRole = role };
                        dashboard.Show();
                        this.Close();
                    }
                    else txtError.Text = "Invalid Username or Password!";
                }
                catch (SqlException ex)
                {
                    txtError.Text = "Database error: " + ex.Message;
                }
            };

            var btnSwitch = new Button { Content = "New User? Register", Height = 30, Background = System.Windows.Media.Brushes.Transparent, Foreground = System.Windows.Media.Brushes.Blue, BorderThickness = new Thickness(0), Cursor = Cursors.Hand };
            btnSwitch.Click += (s, e) => SlideToRegister();

            LoginForm.Children.Add(loginTitle);
            LoginForm.Children.Add(txtUsername);
            LoginForm.Children.Add(txtPassword);
            LoginForm.Children.Add(btnLogin);
            LoginForm.Children.Add(txtError);
            LoginForm.Children.Add(btnSwitch);

            // ------------------ REGISTER FORM ------------------
            RegisterForm.Children.Clear();

            var regTitle = new TextBlock { Text = "Register User", FontSize = 24, FontWeight = FontWeights.Bold, Foreground = System.Windows.Media.Brushes.DarkBlue, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 0, 0, 10) };

            var txtFullName = new TextBox { Height = 35, Margin = new Thickness(0, 10, 0, 5), Style = (Style)FindResource("FocusTextBox") };
            txtFullName.ToolTip = "Enter your full name";

            var txtRegUsername = new TextBox { Height = 35, Margin = new Thickness(0, 10, 0, 5), Style = (Style)FindResource("FocusTextBox") };
            txtRegUsername.ToolTip = "Choose a username";

            var txtEmail = new TextBox { Height = 35, Margin = new Thickness(0, 10, 0, 5), Style = (Style)FindResource("FocusTextBox") };
            txtEmail.ToolTip = "Enter your email";

            var txtRegPassword = new PasswordBox { Height = 35, Margin = new Thickness(0, 10, 0, 5), Style = (Style)FindResource("FocusPasswordBox") };
            txtRegPassword.ToolTip = "Create a strong password";

            var passwordStrength = new ProgressBar { Height = 5, Margin = new Thickness(0, 2, 0, 5), Minimum = 0, Maximum = 100 };
            txtRegPassword.PasswordChanged += (s, e) => { passwordStrength.Value = CalculateStrength(txtRegPassword.Password); };

            var txtRegError = new TextBlock { Foreground = System.Windows.Media.Brushes.Red, Margin = new Thickness(0, 10, 0, 0), TextAlignment = TextAlignment.Center };

            var btnRegister = new Button { Content = "Register", Style = (Style)FindResource("PrimaryButton") };
            btnRegister.Click += (s, e) =>
            {
                txtRegError.Text = "";
                string fullName = txtFullName.Text.Trim();
                string username = txtRegUsername.Text.Trim();
                string email = txtEmail.Text.Trim();
                string password = txtRegPassword.Password.Trim();

                if (string.IsNullOrEmpty(fullName)) { txtRegError.Text = "Full Name required"; return; }
                if (string.IsNullOrEmpty(username)) { txtRegError.Text = "Username required"; return; }
                if (string.IsNullOrEmpty(email)) { txtRegError.Text = "Email required"; return; }
                if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) { txtRegError.Text = "Invalid email format"; return; }
                if (string.IsNullOrEmpty(password)) { txtRegError.Text = "Password required"; return; }

                try
                {
                    string query = "INSERT INTO Users (FullName, Username, Email, Password, Role) VALUES (@f,@u,@e,@p,@r)";
                    SqlParameter[] parameters =
                    {
                        new SqlParameter("@f", fullName),
                        new SqlParameter("@u", username),
                        new SqlParameter("@e", email),
                        new SqlParameter("@p", password),
                        new SqlParameter("@r", "User")
                    };
                    int rows = db.ExecuteNonQuery(query, parameters);
                    if (rows > 0)
                    {
                        MessageBox.Show("Registration successful! Please login.");
                        SlideToLogin();
                    }
                }
                catch (SqlException ex)
                {
                    txtRegError.Text = "Database Error: " + ex.Message;
                }
            };

            var btnSwitchLogin = new Button { Content = "Already have an account? Login", Height = 30, Background = System.Windows.Media.Brushes.Transparent, Foreground = System.Windows.Media.Brushes.Blue, BorderThickness = new Thickness(0), Cursor = Cursors.Hand };
            btnSwitchLogin.Click += (s, e) => SlideToLogin();

            RegisterForm.Children.Add(regTitle);
            RegisterForm.Children.Add(txtFullName);
            RegisterForm.Children.Add(txtRegUsername);
            RegisterForm.Children.Add(txtEmail);
            RegisterForm.Children.Add(txtRegPassword);
            RegisterForm.Children.Add(passwordStrength);
            RegisterForm.Children.Add(btnRegister);
            RegisterForm.Children.Add(txtRegError);
            RegisterForm.Children.Add(btnSwitchLogin);
        }

        private double CalculateStrength(string password)
        {
            int score = 0;
            if (password.Length >= 6) score += 30;
            if (Regex.IsMatch(password, @"[a-z]")) score += 15;
            if (Regex.IsMatch(password, @"[A-Z]")) score += 20;
            if (Regex.IsMatch(password, @"[0-9]")) score += 20;
            if (Regex.IsMatch(password, @"[^\w]")) score += 15;
            return Math.Min(score, 100);
        }

        private void ShowLoginForm() => FormsContainer.RenderTransform = new TranslateTransform(0, 0);
        private void ShowRegisterForm() => FormsContainer.RenderTransform = new TranslateTransform(-320, 0);

        private void SlideToRegister() => AnimateSlide(-320);
        private void SlideToLogin() => AnimateSlide(0);

        private void AnimateSlide(double to)
        {
            TranslateTransform trans = FormsContainer.RenderTransform as TranslateTransform;
            if (trans == null)
            {
                trans = new TranslateTransform();
                FormsContainer.RenderTransform = trans;
            }
            DoubleAnimation anim = new DoubleAnimation { To = to, Duration = TimeSpan.FromMilliseconds(400), EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut } };
            trans.BeginAnimation(TranslateTransform.XProperty, anim);
        }
    }
}
