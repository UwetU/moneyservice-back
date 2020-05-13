using System.Collections.Generic;
using MoneyService.Entities;

namespace MoneyService.Models.Users
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        
        public List<Account> Accounts { get; set; }
        public  List<Transaction> Transactions { get; set; }
    }
}