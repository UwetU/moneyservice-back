using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MoneyService.Entities;
using MoneyService.Helpers;

namespace MoneyService.Services
{
    public interface IUserService
    {
        User Authenticate(string email, string password);
        IEnumerable<User> GetAll();
        User GetById(int id);
        Account CreateAccount(Account account);
        IEnumerable<Account> GetUserAccounts(int userId);
        IEnumerable<Transaction> GetUserTransactions(int userId);
        Transaction Refill(Transaction account);
        User Create(User user, string password);
        void Update(User userParam, string password = null);
        void Delete(int id);
    }
    public class UserService : IUserService
    {
        private DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public User Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Users.SingleOrDefault(x => x.Email == email);

            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }
        
        public User GetById(int id)
        {
            return _context.Users
                .Include(x => x.Accounts)
                .Include(x => x.Transactions)
                .FirstOrDefault(x=> x.Id == id );
        }

        public User Create(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Введите корректный пароль");
            
            if (_context.Users.Any(x => x.Email == user.Email))
                throw new AppException("Пользователь с таким адресом уже существует");

            string passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public void Update(User userParam, string password = null)
        {
            var user = _context.Users.Find(userParam.Id);
            
            if (user == null)
                throw new AppException("Пользователь не найден");

            if (!string.IsNullOrWhiteSpace(userParam.Email) && userParam.Email != user.Email)
            {
                if (_context.Users.Any(x => x.Email == userParam.Email))
                    throw new AppException("Пользователь с таким адресом уже существует");

                user.Email = userParam.Email;
            }

            if (!string.IsNullOrWhiteSpace(userParam.FirstName))
                user.FirstName = userParam.FirstName;

            if (!string.IsNullOrWhiteSpace(userParam.LastName))
                user.LastName = userParam.LastName;

            if (!string.IsNullOrWhiteSpace(password))
            {
                string passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        public Account CreateAccount(Account account)
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

        public Transaction Refill(Transaction transaction)
        {
            if (transaction.NumberAccountOne == null)
                throw new AppException("Введите номер счета");
            
            var account1 = _context.Accounts.FirstOrDefault(x => x.Number == transaction.NumberAccountOne);
            var account2 = _context.Accounts.FirstOrDefault(x => x.Number == transaction.NumberAccountTwo);
            
            if (account1 == null)
                throw new AppException("Счета с таким номером не существует");
            
            if (account2 == null)
                account1.Balance += transaction.SumTransfer;
            else
            {
                if (transaction.SumTransfer > account1.Balance)
                    throw new AppException("На счету недостаточно средств");
                account1.Balance -= transaction.SumTransfer;
                account2.Balance += transaction.SumTransfer;
            }
            
            transaction.UserId = account1.Id;
            transaction.Date = DateTime.Now;

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            return transaction;
        }

        public IEnumerable<Account> GetUserAccounts(int userId)
        {
            return _context.Accounts.Where(x => x.UserId == userId);
        }

        public IEnumerable<Transaction> GetUserTransactions(int userId)
        {
            return _context.Transactions.Where(x => x.UserId == userId);
        }

        private static void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            passwordSalt = Guid.NewGuid().ToString().Substring(0, 8);
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(string.Concat(password, passwordSalt));
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                passwordHash = sb.ToString();
            }
        }

        private static bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(string.Concat(password, storedSalt));
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                
                if (sb.ToString() == storedHash) return true;
                
                return false;
            }
        }
    }
}