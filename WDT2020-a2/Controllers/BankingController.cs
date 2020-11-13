using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WDT2020_a2.Attributes;
using WDT2020_a2.Data;
using WDT2020_a2.Models;
using WDT2020_a2.Services;
using WDT2020_a2.Utilities;

namespace WDT2020_a2.Controllers
{
    [AuthoriseCustomer]
    public class BankingController : Controller
    {
        private readonly BankEngine _bankEngine;

        private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

        public BankingController(NwabContext context)
        {
            _bankEngine = new BankEngine(context);
        }

        // Accounts View
        public async Task<IActionResult> Index() => View(await _bankEngine.GetCustomerWithAccounts(CustomerID));

        // GET
        // Default ATM Form
        public async Task<IActionResult> ATM()
        {
            // intialize variables for populating form
            var customer = await _bankEngine.GetCustomerWithAccounts(CustomerID);
            ViewBag.CustAccounts = customer.Accounts;
            ViewBag.AllAccounts = await _bankEngine.GetAllAccounts();

            return View();
        }

        // Form submission and validation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ATM(Transaction transaction)
        {
            // Initialize variables for populating form
            var customer = await _bankEngine.GetCustomerWithAccounts(CustomerID);
            ViewBag.CustAccounts = customer.Accounts;
            ViewBag.AllAccounts = await _bankEngine.GetAllAccounts();

            // reset destination account to 0 if type is not transfer
            if (transaction.TransactionType != 'T')
                transaction.DestAccount = 0;

            // Validation //
            // amount is positive
            if (transaction.Amount <= 0)
                ModelState.AddModelError(nameof(transaction.Amount), "Amount must be positive");

            // amount has more than 2 decimal places
            if (transaction.Amount.MoreThan2DecimalPlaces())
                ModelState.AddModelError(nameof(transaction.Amount), "Amount cannot have more than 2 decimal places");

            // accNo and destNo can't be same
            if (transaction.AccountNumber == transaction.DestAccount)
                ModelState.AddModelError(nameof(transaction.DestAccount), "Cannot transfer into same account. Try a deposit instead.");

            // account balance is sufficient for transaction
            if (!_bankEngine.AccountHasBalanceForTransaction(transaction.AccountNumber, transaction))
                ModelState.AddModelError(nameof(transaction.Amount), "Account balance too low for transaction.");

            if (!ModelState.IsValid)
                return View(transaction);

            // process transaction, return transactionID for receipt
            int id = _bankEngine.ProcessTransaction(transaction);

            HttpContext.Session.SetInt32(nameof(Transaction.TransactionID), id);

            return RedirectToAction(nameof(Receipt));
        }

        // Receipt of successful transaction
        public async Task<IActionResult> Receipt()
        {
            int transactionID = HttpContext.Session.GetInt32(nameof(Transaction.TransactionID)).Value;

            Transaction transaction = await _bankEngine.GetTransaction(transactionID);

            return View(transaction);
        }
    }
}
