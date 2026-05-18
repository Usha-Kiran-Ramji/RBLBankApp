namespace RBLBankApp.Models
{
    public class Account
    {
        public int AccountID { get; set; }
        public int UserID { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
    }
}
