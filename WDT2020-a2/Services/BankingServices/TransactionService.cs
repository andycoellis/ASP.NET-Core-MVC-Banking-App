using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WDT2020_a2.Data;
using WDT2020_a2.Models;
using WDT2020_a2.Utilities;

namespace WDT2020_a2.Services.BankingServices
{
    public class TransactionService
    {
        private const int ID_LENGTH = 4;

        private readonly NwabContext _context;

        public TransactionService(NwabContext context)
        {
            _context = context;
        }

        /// <summary>Handles the processing of all transactions.
        /// Processes through relevant function, returns transactionID for receipt
        /// </summary>
        public int ProcessTransaction(Transaction transaction)
        {
            // generate random transactionID until no matching ID found
            transaction.TransactionID = GenerateUniqueTransactionID();
            // add new transaction time
            transaction.ModifyDate = DateTime.UtcNow;

            if (transaction.TransactionType == (char)TransactionType.Deposit)
                Deposit(transaction);
            if (transaction.TransactionType == (char)TransactionType.Withdrawal)
                Withdraw(transaction);
            if (transaction.TransactionType == (char)TransactionType.Transfer)
                Transfer(transaction);

            return transaction.TransactionID;
        }

        /// <summary>
        /// Processes deposit transactions
        /// </summary>
        private void Deposit(Transaction deposit)
        {
            // retrieve account with transactions
            var account = GetAccountWithTransactions(deposit.AccountNumber);

            account.Balance += deposit.Amount;

            // add transaction to account
            account.Transactions.Add(deposit);

            // update database
            _context.SaveChanges();
        }

        /// <summary>
        /// Processes withdrawal transactions
        /// </summary>
        private void Withdraw(Transaction withdrawal)
        {
            // retrieve account with transactions
            var account = GetAccountWithTransactions(withdrawal.AccountNumber);

            bool IsFree = IsTransactionFree(withdrawal.AccountNumber);

            double amount = withdrawal.Amount;

            // if transaction is not free, update balance with service charge added
            account.Balance -= IsFree ? amount : amount + (amount / 10);

            account.Transactions.Add(withdrawal);

            // add service charge if withdrawal is not free
            if(!IsFree)
            {
                account.Transactions.Add(new Transaction
                {
                    TransactionID = GenerateUniqueTransactionID(),
                    TransactionType = (char)TransactionType.ServiceCharge,
                    AccountNumber = account.AccountNumber,   
                    // amount is 0.1 of withdrawal per business rules
                    Amount = withdrawal.Amount / 10,
                    // use transactionID of withdrawal in comment
                    Comment = $"Service Charge of withdrawal {withdrawal.TransactionID}",
                    ModifyDate = DateTime.UtcNow
                });
            }

            // update database
            _context.SaveChanges();
        }

        /// <summary>
        /// Processes transfer transactions
        /// </summary>
        private void Transfer(Transaction transfer)
        {
            // retrieve origin account
            var originAccount = GetAccountWithTransactions(transfer.AccountNumber);

            bool IsFree = IsTransactionFree(transfer.AccountNumber);

            double amount = transfer.Amount;

            // if transaction is not free, update balance with service charge added
            originAccount.Balance -= IsFree ? amount : amount + (amount / 5);

            originAccount.Transactions.Add(transfer);

            // add service charge if transfer is not free
            if(!IsFree)
            {
                originAccount.Transactions.Add(
                    new Transaction
                    {
                        TransactionID = GenerateUniqueTransactionID(),
                        TransactionType = (char)TransactionType.ServiceCharge,
                        AccountNumber = transfer.AccountNumber,
                        // amount is 0.2 of transfer per business rules
                        Amount = amount / 5,
                        // use transactionID of transfer in comment
                        Comment = $"Service Charge of transfer {transfer.TransactionID}",
                        ModifyDate = DateTime.UtcNow
                    });
            }

            // Update Destination Account
            // retrieve destination account
            var destinationAccount = GetAccountWithTransactions(transfer.DestAccount);

            destinationAccount.Balance += transfer.Amount;

            // add to destination account as a deposit with comment including details
            destinationAccount.Transactions.Add(
                new Transaction
                {
                    TransactionID = GenerateUniqueTransactionID(),
                    TransactionType = (char)TransactionType.Deposit,
                    AccountNumber = transfer.DestAccount,
                    Amount = transfer.Amount,
                    Comment = $"Transfer from account {transfer.AccountNumber}",
                    ModifyDate = DateTime.UtcNow
                });

            // update database
            _context.SaveChanges();
        }

        /// <summary>Validates if an account has balance for a transaction.
        /// Verifies balance based on account type, transaction type, and transaction amount.
        /// </summary>
        public bool AccountHasBalanceForTransaction(int accountNumber, Transaction transaction)
        {
            // if Deposit, return true
            if (transaction.TransactionType == (char)TransactionType.Deposit)
                return true;

            bool result = true;

            Account account = GetAccountWithTransactions(accountNumber);

            double charge = transaction.Amount;

            // update total to be charged based on transaction type
            if (!IsTransactionFree(accountNumber))
            {
                if (transaction.TransactionType == (char)TransactionType.Withdrawal)
                    charge += charge / 10;
                if (transaction.TransactionType == (char)TransactionType.Transfer)
                    charge += charge / 5;
            }

            // check for savings account
            if (account.AccountType == 'S' && account.Balance < charge)
                result = false;

            // check for checking account with business rule
            if (account.AccountType == 'C' && (account.Balance - 200) < charge)
                result =  false;

            return result;
        }


        ///<summary>Create a new Transaction given a BillPay object.
        ///An Object is returned to the original callee as to keep updates on a single thread
        ///</summary>
        public Transaction CreateTransfer(BillPay billPay)
        {
            //Retrieve account associated with BillPay entry
            var account = _context.Accounts.FirstOrDefault(x => x.AccountNumber == billPay.AccountNumber);

            var transaction = new Transaction
            {
                TransactionID = GenerateUniqueTransactionID(),
                TransactionType = (char)TransactionType.BillPay,
                AccountNumber = billPay.AccountNumber,
                DestAccount = billPay.PayeeID,
                Amount = billPay.Amount,
                Comment = $"Bill paid to " +
                    $"{_context.Payees.FirstOrDefault(x => x.PayeeID == billPay.PayeeID).PayeeName}",
                ModifyDate = DateTime.UtcNow
            };

            return transaction;
        }

        /// <summary>Returns a specific transaction from the database.
        /// Returns a transaction with passed transactionID
        /// </summary>
        public async Task<Transaction> GetTransaction(int transactionID) => await _context.Transactions.FindAsync(transactionID);

        /// <summary>
        /// Returns an Account with all transactions associated with that account.
        /// </summary>
        private Account GetAccountWithTransactions(int accountNumber) => _context.Accounts.Include(x => x.Transactions).First(x => x.AccountNumber == accountNumber);

        /// <summary>
        /// Returns true if account has any free transactions remaining.
        /// </summary>
        private bool IsTransactionFree(int accountNumber)
        {
            // retrieve all transactions of an account number
            var transactions = _context.Transactions.Where(x => x.AccountNumber == accountNumber);

            int count = 0;
            foreach (Transaction t in transactions)
            {
                // if Withdrawal or Transfer, increment count
                if (t.TransactionType == 'W' || t.TransactionType == 'T')
                    count++;
            }

            return count < 4 ? true : false;
        }

        /// <summary>
        /// Returns a unique int transaction ID, after verifying with database
        /// </summary>
        private int GenerateUniqueTransactionID()
        {
            int uniqueID;
            do
            {
                // repurpose utility functionality
                uniqueID = Convert.ToInt32(UtilityFunctions.GenerateStringID(ID_LENGTH));
            }
            // check generated ID doesn't exist in database already
            while (_context.Transactions.Find(uniqueID) != null);

            return uniqueID;
        }
    }
}
