using System.ComponentModel.DataAnnotations.Schema;
using MoneyService.Entities;

namespace MoneyService.Models.Accounts
{
    public class AccountModel
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public double Balance { get; set; }
        public bool Closing { get; set; }
        
        [ForeignKey("User")]
        public int UserId { get; set; }
    }
}