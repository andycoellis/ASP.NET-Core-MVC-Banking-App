using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WDT2020_a2.Data;
using WDT2020_a2.Models;
using WDT2020_a2.Services.BankingServices;

namespace WDT2020_a2.Services
{
    public class BankEngine
    {
        private readonly CustomerService _customerService;
        private readonly LoginService _loginService;
        private readonly AccountService _accountService;
        private readonly TransactionService _transactionService;
        private readonly BillPayService _billPayService;
        private readonly StateService _stateService;

        public BankEngine(NwabContext context)
        {
            _customerService = new CustomerService(context);
            _loginService = new LoginService(context);
            _accountService = new AccountService(context);
            _transactionService = new TransactionService(context);
            _billPayService = new BillPayService(context, this);
            _stateService = new StateService();
        }


       /*      Customer Logic
        * 
        *--------------------------------------------------------------------*/

        public async Task<Customer> NewCustomer(string name, string phoneNumber)
        {
            return await _customerService.CreateClient(name, phoneNumber);
        }

        /// <summary>Return us user given a CustomerID</summary>
        public async Task<Customer> GetCustomer(int? customerID)
        {
            return await _customerService.GetClient(customerID);
        }


        /// <summary>Search if Customer exists in the database</summary>
        public bool DoesUserExist(int id)
        {
            return _customerService.DoesClientExist(id);
        }

        public async Task<Customer> GetCustomerWithAccounts(int customerID)
        {
            return await _customerService.GetCustomerWithAccounts(customerID);
        }

        ///<summary>Update customer in the database</summary>
        public async Task UpdateCustomer(Customer customer)
        {
            await _customerService.UpdateCustomer(customer);
        }


       /*      Login Logic
        * 
        *--------------------------------------------------------------------*/

        ///<Summary> Generate and return and new Login object</Summary>
        public async Task NewLogin(string password, int customerID)
        {
            await _loginService.CreateClient(password, customerID);
        }

        public bool DoesLoginExist(string id)
        {
            return _loginService.DoesClientExist(Convert.ToInt32(id));
        }


        /*      Account Logic
         * 
         *--------------------------------------------------------------------*/

        /// <summary>Return an account given an AccountNumber</summary>
        public async Task<Account> GetAccount(int accountNumber)
        {
            return await _accountService.GetAccount(accountNumber);
        }

        /// <summary>Return a list of all accounts in database</summary>
        public async Task<List<Account>> GetAllAccounts()
        {
            return await _accountService.GetAllAccounts();
        }

        /// <summary>Return a list of all transactions of an account</summary>
        public List<Transaction> GetTransactionsOfAccount(int accountNumber)
        {
            return _accountService.GetTransactionsOfAccount(accountNumber);
        }


        /*      Transaction Logic
         * 
         *--------------------------------------------------------------------*/

        /// <summary>Submits a validated transaction for processing</summary>
        public int ProcessTransaction(Transaction transaction)
        {
            return _transactionService.ProcessTransaction(transaction);
        }

        /// <summary>Validates an account has sufficient balance for a transaction</summary>
        public bool AccountHasBalanceForTransaction(int accountNumber, Transaction transaction)
        {
            return _transactionService.AccountHasBalanceForTransaction(accountNumber, transaction);
        }

        /// <summary>Return a transaction of a given TransactionID</summary>
        public async Task<Transaction> GetTransaction(int transactionID)
        {
            return await _transactionService.GetTransaction(transactionID);
        }

        ///<summary>Process a new BillPay Transaction, argument must be a BillPay object</summary>
        public Transaction BillPayTransaction(BillPay billPay)
        {
            return _transactionService.CreateTransfer(billPay);
        }

        /*      BillPay Logic
         * 
         *--------------------------------------------------------------------*/
        ///<summary>Create a new BillPay</summary>
        public async Task<bool> NewBillPay(int payeeID, int accountNumber, double amount, DateTime scheduleDate, char paymentOccurence)
        {
            return await _billPayService.CreateClient(payeeID, accountNumber, amount, scheduleDate, paymentOccurence);
        }

        ///<summary>Return a List<BillPay> that are related to a single customer</summary>
        public List<BillPay> GetBillPays(int customerNumber)
        {
            return _billPayService.GetBills(customerNumber);
        }

        ///<summary>Get a List of all BillPays with attached payee and account information</summary>
        public List<BillPay> GetBillPaysComplete(int customerNumber)
        {
            return _billPayService.GetBillsComplete(customerNumber);
        }

        ///<summary>Return a BillPay associated with a BillPayID and their CustomerID</summary>
        public BillPay GetBillPayWithPayee(int billPayID)
        {
            return _billPayService.GetBillWithPayee(billPayID);
        }

        ///<summary>Return a BillPau given a BillPay ID number</summary>
        public BillPay GetBillPay(int id)
        {
            return _billPayService.GetBill(id);
        }

        ///<summary>Modify a BillPay entry in the database</summary>
        public async Task<bool> ModifyBillPay(BillPay bill)
        {
            return await _billPayService.UpdateBill(bill);
        }

        ///<summary>Delete a BillPay entry in the database</summary>
        public async Task<bool> DeleteBill(int billID)
        {
            return await _billPayService.DeleteBill(billID);
        }

        ///<summary>Process all BillPays that are currently available with designated scheduled time</summary>
        public void ProcessBills()
        {
            _billPayService.ProcessBills();
        }

        ///<summary>Return a list of all accounts associated with a given CustomerID</summary>
        public List<Account> GetAccounts(int customerID)
        {
            return _billPayService.GetAccounts(customerID);
        }


        /*      States Logic
         * 
         *--------------------------------------------------------------------*/

        ///<summary>Return a list of all current Australian states and territories</summary>
        public List<States> GetStates()
        {
            return _stateService.GetList();
        }
    }
}
