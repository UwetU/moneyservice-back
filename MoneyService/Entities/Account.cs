using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyService.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public double Balance { get; set; }
        public bool Closing { get; set; }
        
        [ForeignKey("User")]
        public int UserId { get; set; }
    }
}