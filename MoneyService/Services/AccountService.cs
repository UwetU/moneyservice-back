using System;
using System.Collections.Generic;
using System.Linq;
using MoneyService.Entities;
using MoneyService.Helpers;

namespace MoneyService.Services
{
    public interface IAccountService
    {
        Account Create(Account account);
        IEnumerable<Account> GetUserAccounts(int userId);
        void Close(Account account);
    }
    public class AccountService : IAccountService
    {
        private DataContext _context;

        public AccountService(DataContext context)
        {
            _context = context;
        }

        public Account Create(Account account)
        {
            string firstNumber = "4";
            
            do
            {
                Random rnd = new Random();
                int rndNumber = rnd.Next(100000000, 999999999);
                account.Number = String.Concat(firstNumber, rndNumber.ToString());
            } while (_context.Accounts.Any(x => x.Number == account.Number));

            account.Balance = 0.00;
            account.Closing = false;

            _context.Accounts.Add(account);
            _context.SaveChanges();

            return account;
        }

        public void Close(Account accountParam)
        {
            var account = _context.Accounts.Find(accountParam.Id);
            
            if (account.Closing)
                throw new AppException("Данный счет уже закрыт");
            
            account.Closing = accountParam.Closing;

            _context.Accounts.Update(account);
            _context.SaveChanges();
        }

        public IEnumerable<Account> GetUserAccounts(int userId)
        {
            return _context.Accounts.Where(x => x.UserId == userId);
        }
    }
}