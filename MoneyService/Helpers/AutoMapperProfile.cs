using AutoMapper;
using MoneyService.Entities;
using MoneyService.Models.Accounts;
using MoneyService.Models;
using MoneyService.Models.Users;

namespace MoneyService.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<RegisterModel, User>();
            CreateMap<UpdateModel, User>();
            CreateMap<Account, AccountModel>();
            CreateMap<CloseModel, Account>();
            CreateMap<AccountModel, Account>();
            CreateMap<Transaction, TransactionModel>();
            CreateMap<TransactionModel, Transaction>();
        }
    }
}