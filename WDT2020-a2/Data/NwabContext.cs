using System;
using Microsoft.EntityFrameworkCore;
using WDT2020_a2.Models;

namespace WDT2020_a2.Data
{
    public class NwabContext : DbContext
    {
        public NwabContext(DbContextOptions<NwabContext> options) : base(options)
        { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<BillPay> BillPays { get; set; }
        public DbSet<Payee> Payees { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Constraints
            builder.Entity<Customer>().HasCheckConstraint("CH_Customer_CustomerID", "len(CustomerID) = 4").
                HasCheckConstraint("CH_Customer_PostCode", "len(PostCode) = 4");

            builder.Entity<Login>().HasCheckConstraint("CH_Login_CustomerID", "len(CustomerID) = 4").
                HasCheckConstraint("CH_Login_Password", "len(Password) = 64");

            builder.Entity<Account>().HasCheckConstraint("CH_Account_AccountNumber", "len(AccountNumber) = 4").
            HasCheckConstraint("CH_Account_AccountType", "len(AccountType) = 1");
            builder.Entity<Account>().HasCheckConstraint("CH_Account_AccountType", "AccountType in ('C', 'S')");


            builder.Entity<BillPay>().HasCheckConstraint("CH_BillPay_BillPayID", "len(BillPayID) = 4").
            HasCheckConstraint("CH_BillPay_Period", "len(Period) = 1");
            builder.Entity<BillPay>().HasCheckConstraint("CH_BillPay_Period", "Period in ('M', 'Q', 'A', 'S')");

            builder.Entity<Payee>().HasCheckConstraint("CH_Payee_PayeeID", "len(PayeeID) = 4");

            builder.Entity<Transaction>().HasCheckConstraint("CH_Transaction_TransactionID", "len(TransactionID) = 4").
            HasCheckConstraint("CH_Transaction_TransactionType", "len(TransactionType) = 1");
            builder.Entity<Transaction>().HasCheckConstraint("CH_Transaction_TransactionType", "TransactionType in ('D', 'W', 'T', 'S', 'B')");

            //Relationships
            builder.Entity<Transaction>().
                HasOne(x => x.Account).WithMany(x => x.Transactions).HasForeignKey(x => x.AccountNumber);
            builder.Entity<Transaction>().HasCheckConstraint("CH_Transaction_Amount", "Amount > 0");

            builder.Entity<BillPay>().
                HasOne(x => x.Account).WithMany(x => x.BillPays).HasForeignKey(x => x.AccountNumber);
            builder.Entity<BillPay>().HasCheckConstraint("CH_BillPay_Amount", "Amount > 0");
            builder.Entity<BillPay>().
                HasOne(x => x.Payee).WithMany(x => x.BillPays).HasForeignKey(x => x.PayeeID);
        }
    }
}
