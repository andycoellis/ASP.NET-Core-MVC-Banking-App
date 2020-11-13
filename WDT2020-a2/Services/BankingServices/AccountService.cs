using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WDT2020_a2.Data;
using WDT2020_a2.Models;

namespace WDT2020_a2.Services.BankingServices
{
    public class AccountService
    {
        private readonly NwabContext _context;

        public AccountService(NwabContext context)
        {
            _context = context;
        }

        public async Task<Account> GetAccount(int accountNumber) => await _context.Accounts.FindAsync(accountNumber);

        public async Task<List<Account>> GetAllAccounts() => await _context.Accounts.ToListAsync();

        public List<Transaction> GetTransactionsOfAccount(int accountNumber) => _context.Transactions.Where(x => x.AccountNumber == accountNumber).OrderByDescending(x => x.ModifyDate).ToList();
    }
}
