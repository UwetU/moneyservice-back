using System;
using System.Collections.Generic;
using System.Linq;
using MoneyService.Entities;
using MoneyService.Helpers;

namespace MoneyService.Services
{
    public interface ITransactionService
    {
        IEnumerable<Transaction> GetUserTransactions(int userId);
        Transaction Transfer(Transaction transaction);
        Transaction Refill(Transaction transaction);
    }
    public class TransactionService : ITransactionService
    {
        private DataContext _context;

        public TransactionService(DataContext context)
        {
            _context = context;
        }

        public Transaction Refill(Transaction transaction)
        {
            if (transaction.NumberAccountOne == null)
                throw new AppException("Введите номер счета");

            var account = _context.Accounts.FirstOrDefault(x => x.Number == transaction.NumberAccountOne);
            
            if (account == null)
                throw new AppException("Счета с таким номером не существует");
            
            account.Balance += transaction.SumTransfer;
            
            transaction.UserId = account.Id;
            transaction.Date = DateTime.Now;
            
            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            return transaction;
        }

        public Transaction Transfer(Transaction transaction)
        {
            if (transaction.NumberAccountOne == null || transaction.NumberAccountTwo == null)
                throw new AppException("Введите номер счета");
            
            var account1 = _context.Accounts.FirstOrDefault(x => x.Number == transaction.NumberAccountOne);
            var account2 = _context.Accounts.FirstOrDefault(x => x.Number == transaction.NumberAccountTwo);
            
            if (account1 == null || account2 == null)
                throw new AppException("Счета с таким номером не существует");
            
            if (transaction.SumTransfer > account1.Balance)
                throw new AppException("На счету недостаточно средств");
            
            account1.Balance -= transaction.SumTransfer;
            account2.Balance += transaction.SumTransfer;
            
            transaction.UserId = account1.Id;
            transaction.Date = DateTime.Now;
            
            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            return transaction;
        }

        public IEnumerable<Transaction> GetUserTransactions(int userId)
        {
            return _context.Transactions.Where(x => x.UserId == userId);
        }
    }
}