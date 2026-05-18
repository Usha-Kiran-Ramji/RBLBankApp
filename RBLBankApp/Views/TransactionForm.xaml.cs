// ===============================================
// FILE: Views/TransactionForm.xaml.cs
// ===============================================

using System;
using System.Data.SqlClient;
using System.Windows;
using RBLBankApp.Services;

namespace RBLBankApp.Views
{
    public partial class TransactionForm : Window
    {
        DatabaseService db = new DatabaseService();

        public TransactionForm()
        {
            InitializeComponent();
        }

        // =========================================================
        // TRANSFER MONEY
        // =========================================================
        // Definition:
        // Transfers amount between accounts.
        //
        // Approach:
        // 1. Validate fields
        // 2. Check sender exists
        // 3. Check receiver exists
        // 4. Check balance
        // 5. Update balances
        // 6. Insert transaction
        //
        // Time Complexity:
        // O(1)
        // =========================================================

        private void Transfer_Click(object sender, RoutedEventArgs e)
        {
            string from = txtFromAccount.Text.Trim();
            string to = txtToAccount.Text.Trim();

            if (!decimal.TryParse(txtAmount.Text, out decimal amount))
            {
                MessageBox.Show("Invalid Amount");
                return;
            }

            try
            {
                // CHECK SENDER ACCOUNT

                int senderCount = Convert.ToInt32(
                    db.ExecuteScalar(
                        "SELECT COUNT(*) FROM Accounts WHERE AccountNumber=@a",
                        new SqlParameter[]
                        {
                            new SqlParameter("@a", from)
                        }));

                if (senderCount == 0)
                {
                    MessageBox.Show("Sender account not found");
                    return;
                }

                // CHECK RECEIVER ACCOUNT

                int receiverCount = Convert.ToInt32(
                    db.ExecuteScalar(
                        "SELECT COUNT(*) FROM Accounts WHERE AccountNumber=@a",
                        new SqlParameter[]
                        {
                            new SqlParameter("@a", to)
                        }));

                if (receiverCount == 0)
                {
                    MessageBox.Show("Receiver account not found");
                    return;
                }

                // CHECK BALANCE

                decimal senderBalance = Convert.ToDecimal(
                    db.ExecuteScalar(
                        "SELECT Balance FROM Accounts WHERE AccountNumber=@a",
                        new SqlParameter[]
                        {
                            new SqlParameter("@a", from)
                        }));

                if (senderBalance < amount)
                {
                    MessageBox.Show("Insufficient Balance");
                    return;
                }

                // UPDATE SENDER

                db.ExecuteNonQuery(
                    "UPDATE Accounts SET Balance = Balance - @amt WHERE AccountNumber=@a",
                    new SqlParameter[]
                    {
                        new SqlParameter("@amt", amount),
                        new SqlParameter("@a", from)
                    });

                // UPDATE RECEIVER

                db.ExecuteNonQuery(
                    "UPDATE Accounts SET Balance = Balance + @amt WHERE AccountNumber=@a",
                    new SqlParameter[]
                    {
                        new SqlParameter("@amt", amount),
                        new SqlParameter("@a", to)
                    });

                // INSERT TRANSACTION

                db.ExecuteNonQuery(
                    @"INSERT INTO Transactions
                    (FromAccount, ToAccount, Amount, TransactionType, TransactionDate)
                    VALUES
                    (@f,@t,@amt,'Transfer',GETDATE())",

                    new SqlParameter[]
                    {
                        new SqlParameter("@f", from),
                        new SqlParameter("@t", to),
                        new SqlParameter("@amt", amount)
                    });

                MessageBox.Show("Transfer Successful!");

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}