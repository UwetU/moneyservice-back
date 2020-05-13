using System.Collections.Generic;

namespace MoneyService.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        
        public List<Account> Accounts { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}