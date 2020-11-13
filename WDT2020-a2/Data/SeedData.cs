using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using WDT2020_a2.Models;


namespace WDT2020_a2.Data
{

    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var dummy = new NwabContext(serviceProvider.GetRequiredService<DbContextOptions<NwabContext>>());

            // Look for customers.
            if (dummy.Customers.Any())
                return; // DB has already been seeded.

            dummy.Customers.AddRange(
                new Customer
                {
                    CustomerID = 2100,
                    CustomerName = "Matthew Bolger",
                    Address = "123 Fake Street",
                    City = "Melbourne",
                    PostCode = "3000",
                    Phone = "0421888888"
                },
                new Customer
                {
                    CustomerID = 2200,
                    CustomerName = "Rodney Cocker",
                    Address = "456 Real Road",
                    City = "Melbourne",
                    PostCode = "3005",
                    Phone = "0422102938"
                },
                new Customer
                {
                    CustomerID = 2300,
                    CustomerName = "Shekhar Kalra",
                    Phone = "0123456789"
                });

            dummy.Logins.AddRange(
                new Login
                {
                    UserID = "12345678",
                    CustomerID = 2100,
                    Password = "YBNbEL4Lk8yMEWxiKkGBeoILHTU7WZ9n8jJSy8TNx0DAzNEFVsIVNRktiQV+I8d2",
                    ModifyDate = DateTime.Now.ToUniversalTime()
                },
                new Login
                {
                    UserID = "38074569",
                    CustomerID = 2200,
                    Password = "EehwB3qMkWImf/fQPlhcka6pBMZBLlPWyiDW6NLkAh4ZFu2KNDQKONxElNsg7V04",
                    ModifyDate = DateTime.Now.ToUniversalTime()
                },
                new Login
                {
                    UserID = "17963428",
                    CustomerID = 2300,
                    Password = "LuiVJWbY4A3y1SilhMU5P00K54cGEvClx5Y+xWHq7VpyIUe5fe7m+WeI0iwid7GE",
                    ModifyDate = DateTime.Now.ToUniversalTime()
                });

            dummy.Accounts.AddRange(
                new Account
                {
                    AccountNumber = 4100,
                    AccountType = (char)AccountType.Saving,
                    CustomerID = 2100,
                    Balance = 5000,
                    ModifyDate = DateTime.Now.ToUniversalTime()
                },
                new Account
                {
                    AccountNumber = 4101,
                    AccountType = (char)AccountType.Checking,
                    CustomerID = 2100,
                    Balance = 5000,
                    ModifyDate = DateTime.Now.ToUniversalTime()
                },
                new Account
                {
                    AccountNumber = 4200,
                    AccountType = (char)AccountType.Saving,
                    CustomerID = 2200,
                    Balance = 5000,
                    ModifyDate = DateTime.Now.ToUniversalTime()
                },
                new Account
                {
                    AccountNumber = 4300,
                    AccountType = (char)AccountType.Checking,
                    CustomerID = 2300,
                    Balance = 1250.50,
                    ModifyDate = DateTime.Now.ToUniversalTime()
                });

            const string openingBalance = "Opening balance";
            const string format = "dd/MM/yyyy hh:mm:ss tt";
            dummy.Transactions.AddRange(
                new Transaction
                {
                    TransactionID = 1111,
                    TransactionType = (char)TransactionType.Deposit,
                    AccountNumber = 4100,
                    Amount = 100,
                    Comment = openingBalance,
                    ModifyDate = DateTime.ParseExact("19/12/2019 08:00:00 PM", format, null).ToUniversalTime()
                },
                new Transaction
                {
                    TransactionID = 2222,
                    TransactionType = (char)TransactionType.Deposit,
                    AccountNumber = 4101,
                    Amount = 500,
                    Comment = openingBalance,
                    ModifyDate = DateTime.ParseExact("19/12/2019 08:30:00 PM", format, null).ToUniversalTime()
                },
                new Transaction
                {
                    TransactionID = 3333,
                    TransactionType = (char)TransactionType.Deposit,
                    AccountNumber = 4200,
                    Amount = 500.95,
                    Comment = openingBalance,
                    ModifyDate = DateTime.ParseExact("19/12/2019 09:00:00 PM", format, null).ToUniversalTime()
                },
                new Transaction
                {
                    TransactionID = 4444,
                    TransactionType = (char)TransactionType.Deposit,
                    AccountNumber = 4300,
                    Amount = 1250.50,
                    Comment = "Opening balance",
                    ModifyDate = DateTime.ParseExact("19/12/2019 10:00:00 PM", format, null).ToUniversalTime()
                });

            dummy.Payees.AddRange(
                new Payee
                {
                    PayeeID = 1212,
                    PayeeName = "Telstra",
                    Address = "242 Exhibition Street",
                    City = "Melbourne",
                    State = "VIC",
                    PostCode = "3000",
                    Phone = "039933221"
                },
                new Payee
                {
                    PayeeID = 1313,
                    PayeeName = "Energy Australia",
                    Address = "300 Russell Street",
                    City = "Melbourne",
                    State = "VIC",
                    PostCode = "3000",
                    Phone = "039933111"
                },
                new Payee
                {
                    PayeeID = 1414,
                    PayeeName = "Gas Corp",
                    Address = "41 Clarendon St",
                    City = "East Melbourne",
                    State = "VIC",
                    PostCode = "3050",
                    Phone = "038833221"
                },
                new Payee
                {
                    PayeeID = 1515,
                    PayeeName = "Mob Boss",
                    Address = "34 Nunya Bussiness Court",
                    City = "Off Grid",
                    State = "VIC",
                    PostCode = "3666",
                    Phone = "039955221"
                },
                new Payee
                {
                    PayeeID = 1616,
                    PayeeName = "ATO",
                    Address = "33 Debt Alley",
                    City = "Canberra",
                    State = "ACT",
                    PostCode = "5001",
                    Phone = "039937771"
                },
                new Payee
                {
                    PayeeID = 1717,
                    PayeeName = "Medibank",
                    Address = "300 Colins Street",
                    City = "Melbourne",
                    State = "VIC",
                    PostCode = "3000",
                    Phone = "039933999"
                });

            dummy.BillPays.AddRange(
                new BillPay
                {
                    BillPayID = 1111,
                    AccountNumber = 4100,
                    PayeeID = 1212,
                    Amount = 300,
                    ScheduleDate = DateTime.ParseExact("30/01/2020 12:00:00 PM", format, null),
                    Period = char.Parse(nameof(PeriodLength.S)),
                    ModifyDate = DateTime.Now
                },
                new BillPay
                {
                    BillPayID = 1112,
                    AccountNumber = 4100,
                    PayeeID = 1313,
                    Amount = 12.5,
                    ScheduleDate = DateTime.ParseExact("03/02/2020 12:10:00 PM", format, null),
                    Period = char.Parse(nameof(PeriodLength.M)),
                    ModifyDate = DateTime.Now
                },
                new BillPay
                {
                    BillPayID = 1113,
                    AccountNumber = 4101,
                    PayeeID = 1414,
                    Amount = 30,
                    ScheduleDate = DateTime.ParseExact("28/01/2020 02:20:00 PM", format, null),
                    Period = char.Parse(nameof(PeriodLength.M)),
                    ModifyDate = DateTime.Now
                },
                new BillPay
                {
                    BillPayID = 1114,
                    AccountNumber = 4101,
                    PayeeID = 1515,
                    Amount = 100,
                    ScheduleDate = DateTime.ParseExact("28/01/2020 02:10:00 PM", format, null),
                    Period = char.Parse(nameof(PeriodLength.S)),
                    ModifyDate = DateTime.Now
                },
                new BillPay
                {
                    BillPayID = 1115,
                    AccountNumber = 4100,
                    PayeeID = 1616,
                    Amount = 50,
                    ScheduleDate = DateTime.ParseExact("28/01/2020 08:20:00 PM", format, null),
                    Period = char.Parse(nameof(PeriodLength.A)),
                    ModifyDate = DateTime.Now
                },
                new BillPay
                {
                    BillPayID = 1116,
                    AccountNumber = 4100,
                    PayeeID = 1717,
                    Amount = 80,
                    ScheduleDate = DateTime.ParseExact("27/01/2020 05:00:00 PM", format, null),
                    Period = char.Parse(nameof(PeriodLength.Q)),
                    ModifyDate = DateTime.Now
                },
                new BillPay
                {
                    BillPayID = 1117,
                    AccountNumber = 4200,
                    PayeeID = 1515,
                    Amount = 100,
                    ScheduleDate = DateTime.ParseExact("28/01/2020 02:10:00 PM", format, null),
                    Period = char.Parse(nameof(PeriodLength.S)),
                    ModifyDate = DateTime.Now
                },
                new BillPay
                {
                    BillPayID = 1118,
                    AccountNumber = 4200,
                    PayeeID = 1616,
                    Amount = 50,
                    ScheduleDate = DateTime.ParseExact("28/01/2020 08:20:00 PM", format, null),
                    Period = char.Parse(nameof(PeriodLength.A)),
                    ModifyDate = DateTime.Now
                },
                new BillPay
                {
                    BillPayID = 1119,
                    AccountNumber = 4200,
                    PayeeID = 1313,
                    Amount = 80,
                    ScheduleDate = DateTime.ParseExact("29/01/2020 05:00:00 PM", format, null),
                    Period = char.Parse(nameof(PeriodLength.Q)),
                    ModifyDate = DateTime.Now
                });

            dummy.SaveChanges();
        }
    }

}
