using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WDT2020_a2.Data;
using WDT2020_a2.Exceptions;
using WDT2020_a2.Models;
using WDT2020_a2.Utilities;

namespace WDT2020_a2.Services.BankingServices
{
    public class BillPayService
    {
        private const int ID_LENGTH = 4;

        private readonly NwabContext _context;
        private readonly BankEngine _engine;


        public BillPayService(NwabContext context, BankEngine engine)
        {
            _context = context;
            _engine = engine;
        }

        ///<summary>Create a new BillPay object</summary>
        public async Task<bool> CreateClient(int payeeID, int accountNumber, double amount, DateTime scheduleDate, char paymentOccurence)
        {
            string id = "";

            do
            {
                id = UtilityFunctions.GenerateStringID(ID_LENGTH);
                //Search if the new identification number already exists in the database
            } while (_context.BillPays.FirstOrDefault(x => x.BillPayID == Convert.ToInt32(id)) != null);

            var billPay = new BillPay
            {
                BillPayID = Convert.ToInt32(id),
                PayeeID = payeeID,
                AccountNumber = accountNumber,
                Amount = amount,
                ScheduleDate = scheduleDate,
                Period = paymentOccurence,
                ModifyDate = DateTime.Now
            };

            try
            {
                _context.Add(billPay);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                    return true;
            }
            catch(Exception e)
            {
                throw new CustomDatabaseException(nameof(BillPay), e.Message);
            }

            return false;
        }

        ///<summary>Return a list of all standing BillPays</summary>
        public List<BillPay> GetBills(int customerNumber)
        {
            var standingBills = new List<BillPay>();

            var billPay = from b in _context.Accounts
                          where b.CustomerID == customerNumber
                          select b.BillPays;

            standingBills.AddRange(from list in billPay
                                   from bill in list
                                   select bill);
            return standingBills;
        }

        ///<summary>Return a list of BillPays with payee and account information attached</summary>
        public List<BillPay> GetBillsComplete(int customerNumber)
        {            
            using(var context = _context)
            {
                //Return a joined table of bills that are currently associated with with payees
                var list = from bills in context.BillPays
                           join payees in context.Payees
                           on bills.PayeeID equals payees.PayeeID
                           select bills;

                //Narrow previous table by joining accounts only associated with method argument customer account
                //This now shows only BillPays that are designated with a specific customer
                var result = from items in list
                             join acc in context.Accounts
                             on items.AccountNumber equals acc.AccountNumber
                             where acc.CustomerID == customerNumber
                             select items;

                List<BillPay> newList = new List<BillPay>();

                //Create a new list from DBContext, now including payee information with each BillPay
                //Only attaining Bills that are associated with BillPay IDs found in the above table
                foreach (var item in result)
                {
                    newList.Add(
                        context.BillPays
                        .Include(bill => bill.Payee)
                        .FirstOrDefault(bill => bill.BillPayID == item.BillPayID)
                        );
                }

                return newList;
            }
        }

        ///<summary>Return a list of BillPays with payee and account information attached</summary>
        public BillPay GetBillWithPayee(int billID)
        {
            using (var context = _context)
            {
                var bill = context.BillPays.Include(bill => bill.Payee)
                    .FirstOrDefault(bill => bill.BillPayID == billID);

                return bill;
            }
        }


        ///<summary>Return a BillPay given a BillPayID</summary>
        public BillPay GetBill(int billID)
        {
            return _context.BillPays.FirstOrDefault(x => x.BillPayID == billID);
        }

        ///<summary>Update BillPay entry into database, argument should be a newly submitted Bill with relevant ID</summary>
        public async Task<bool> UpdateBill(BillPay bill)
        {

            if (bill == null)
                throw new CustomDatabaseException("This BillPayID does not exist.");

            bill.ModifyDate = DateTime.Now;

            bool response = false;

            try
            {
                _context.Update(bill);
                var entriesChanged = await _context.SaveChangesAsync();

                if (entriesChanged > 0)
                    response = true;
            }
            catch(Exception e)
            {
                throw new CustomDatabaseException(nameof(BillPay), e.Message);
            }

            return response;
        }

        ///<summary>Delete a BillPay entry in the database given its BillPayID</summary>
        public async Task<bool> DeleteBill(int billID)
        {
            bool status = false;

            var entry = GetBill(billID);

            try
            {
                _context.Remove(entry);
                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                throw new CustomDatabaseException(nameof(BillPay), e.Message);
            }

            var check = GetBill(billID);

            if (check == null)
                status = true;

            return status;
        }

        ///<summary>
        ///Iterates over all BillPays stored in Database processing
        ///them based on schedule date.
        ///</summary>
        public void ProcessBills()
        {          

            //Get all BillPays where their schedule date has passed the due date
            var dbBillPay = from b in _context.BillPays
                        where DateTime.Compare(DateTime.Now, b.ScheduleDate) > 0
                        select b;

            //Transfer Explict call to a list then deallocate the memory
            var billsList = dbBillPay.ToList();
            dbBillPay = default;
                

            foreach (var bill in billsList)
            {
                //Return the account associated with the BillPay in order to check available funds
                var account =
                    _context.Accounts.FirstOrDefault(
                        x => x.AccountNumber == bill.AccountNumber);

                //If there are no available funds to process the transaction then the BillPay entry is deleted and
                //a warning is thrown, Exception is intended to be logged in Ilogger.
                if (account.Balance < bill.Amount)
                {
                    _context.Remove(bill);
                    _context.SaveChanges();

                    throw new CustomTransactionException(bill.AccountNumber.ToString(), bill.BillPayID.ToString(),
                        "Not enough funds to process transaction. BillPay has been cancelled");
                }

                //If it is a single (one-off) Bill then process transaction and delete Bill Entry
                if (bill.Period.ToString().Equals(nameof(PeriodLength.S)))
                {
                    var transaction = _engine.BillPayTransaction(bill);

                    account.Balance -= transaction.Amount;
                    account.ModifyDate = DateTime.UtcNow;

                    _context.Add(transaction);
                    _context.Remove(bill);
                }

                //Process all other transactions and adjust their schedule time to the period labelled
                else
                {
                    PeriodLength increment;

                    if (bill.Period.ToString().Equals(nameof(PeriodLength.M)))
                        increment = PeriodLength.M;
                    else if (bill.Period.ToString().Equals(nameof(PeriodLength.Q)))
                        increment = PeriodLength.Q;
                    else
                        increment = PeriodLength.A;

                    var transaction = _engine.BillPayTransaction(bill);

                    //Debit BillPay amount from account
                    account.Balance -= transaction.Amount;
                    account.ModifyDate = DateTime.UtcNow;

                    bill.ScheduleDate = bill.ScheduleDate.AddMonths((int)increment);
                    bill.ModifyDate = DateTime.Now;

                    try
                    {
                        //Add both changes to the database
                        _context.Add(transaction);
                        _context.Update(account);
                        _context.Update(bill);
                    }
                    catch(Exception e)
                    {
                        throw new CustomDatabaseException(nameof(BillPay), e.Message);
                    }
    
                }
            }
            _context.SaveChangesAsync();
        }

        public List<Account> GetAccounts(int customerID)
        {

            var accounts = from accts in _context.Accounts
                           where accts.CustomerID == customerID
                           select accts;

            if (accounts == null)
                throw new CustomDatabaseException(nameof(Account),
                    $"No accounts exist under associated with {customerID}");

            return accounts.ToList();

        }
    }
}
