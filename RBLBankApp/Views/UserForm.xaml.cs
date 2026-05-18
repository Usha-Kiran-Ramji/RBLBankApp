using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using RBLBankApp.Services;

namespace RBLBankApp.Views
{
    public partial class UserForm : Window
    {
        DatabaseService db = new DatabaseService();

        public int UserID = 0; // 0 = Add, >0 = Edit

        public UserForm()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string name = txtFullName.Text;
            string username = txtUsername.Text;
            string email = txtEmail.Text;
            string password = txtPassword.Password;
            string role = (cmbRole.SelectedItem as ComboBoxItem)?.Content.ToString();

            // 🔴 Validation
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Please fill all fields!");
                return;
            }
    
            if (UserID == 0)
            {
                // ➕ INSERT (SAFE)
                string query = @"INSERT INTO Users 
                                (FullName, Username, Password, Email, Role) 
                                VALUES (@name, @username, @password, @email, @role)";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@name", name),
                    new SqlParameter("@username", username),
                    new SqlParameter("@password", password),
                    new SqlParameter("@email", email),
                    new SqlParameter("@role", role)
                };

                db.ExecuteNonQuery(query, parameters);

                MessageBox.Show("User Added Successfully!");
            }
            else
            {
                // ✏️ UPDATE (SAFE)
                string query = @"UPDATE Users SET 
                                FullName=@name,
                                Username=@username,
                                Password=@password,
                                Email=@email,
                                Role=@role
                                WHERE UserID=@id";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@name", name),
                    new SqlParameter("@username", username),
                    new SqlParameter("@password", password),
                    new SqlParameter("@email", email),
                    new SqlParameter("@role", role),
                    new SqlParameter("@id", UserID)
                };

                db.ExecuteNonQuery(query, parameters);

                MessageBox.Show("User Updated Successfully!");
            }

            this.Close();
        }
    }
}
