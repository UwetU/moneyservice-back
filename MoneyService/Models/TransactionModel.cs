using System;
using System.ComponentModel.DataAnnotations.Schema;
using MoneyService.Entities;

namespace MoneyService.Models
{
    public class TransactionModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double SumTransfer { get; set; }
        public string NumberAccountOne { get; set; }
        public string NumberAccountTwo { get; set; }
        
        [ForeignKey("User")]
        public int UserId { get; set; }
    }
}