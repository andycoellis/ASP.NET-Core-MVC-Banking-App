using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WDT2020_a2.Attributes;
using WDT2020_a2.Data;
using WDT2020_a2.Models;
using WDT2020_a2.Services;
using X.PagedList;

namespace WDT2020_a2.Controllers
{
    [AuthoriseCustomer]
    public class AccountsController : Controller
    {
        private readonly BankEngine _bankEngine;

        private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

        public AccountsController(NwabContext context)
        {
            _bankEngine = new BankEngine(context);
        }

        // Accounts View
        public async Task<IActionResult> Index() => View(await _bankEngine.GetCustomerWithAccounts(CustomerID));


        public IActionResult Statement(int id, int? page)
        {
            // code below adapted from https://github.com/dncuug/X.PagedList

            ViewBag.ID = id;

            var transactions = _bankEngine.GetTransactionsOfAccount(id);

            var pageNumber = page ?? 1;

            var onePageOfTransactions = transactions.ToPagedList(pageNumber, 4);

            ViewBag.OnePageOfTransactions = onePageOfTransactions;

            return View();
        }
    }
}
