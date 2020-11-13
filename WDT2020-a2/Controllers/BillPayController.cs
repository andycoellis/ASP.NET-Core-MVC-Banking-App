using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WDT2020_a2.Attributes;
using WDT2020_a2.Data;
using WDT2020_a2.Models;
using WDT2020_a2.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WDT2020_a2.Exceptions;

namespace WDT2020_a2.Controllers
{

    [AuthoriseCustomer]
    [Route("/[Controller]")]
    public class BillPayController : Controller
    {
        private readonly NwabContext _context;
        private readonly BankEngine _engine;
        private readonly ILogger _logger;


        public BillPayController(NwabContext context, ILogger<BillPayController> logger)
        {
            _context = context;
            _logger = logger;

            _engine = new BankEngine(_context);

        }

        [Route("Bills")]
        public IActionResult Index()
        {
            try
            {
                var customerID = GetsessionUser();
                var billList = _engine.GetBillPaysComplete((int)customerID);

                //This list returns all BillPays associated with the Customer in session
                billList = billList.OrderBy(x => x.ScheduleDate).ToList();

                return View(billList);
            }
            catch (CustomDatabaseException e)
            {
                _logger.LogError(e.Message);
            }

            return View();
        }

        [HttpPost]
        [Route("Edit")]
        public async Task<ActionResult> Edit(BillPay bill)
        {
            if (bill.ScheduleDate < DateTime.Now)
                ModelState.AddModelError("CustomError", "Date must not be in the past.");

            if (ModelState.ErrorCount > 1)
                return View(bill);

            try
            {
                var response = await _engine.ModifyBillPay(bill);
                if (!response)
                    return NotFound();

                return RedirectToAction(nameof(Index));

            }
            catch (CustomDatabaseException e)
            {
                _logger.LogError(e.Message);

            }
            catch (CustomTransactionException e)
            {
                _logger.LogCritical(e.Message);
            }

            return Redirect(nameof(Index));
        }

        [Route("Edit")]
        public IActionResult Edit(int id)
        {
            try
            {
                var bill = _engine.GetBillPayWithPayee(id);

                return View(bill);
            }
            catch (CustomDatabaseException e)
            {
                _logger.LogError(e.Message);
            }
            return View(nameof(Index));
        }


        [Route("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _engine.DeleteBill(id);

                if (!response)
                    return NotFound();
            }
            catch(CustomDatabaseException e)
            {
                _logger.LogError($"Database: {e}");
            }
            catch(CustomTransactionException e)
            {
                _logger.LogInformation($"Transaction: {e}");
            }

            return RedirectToAction(nameof(Index));
        }

        [Route("New")]
        public IActionResult Create()
        {
            try
            {
                var custID = GetsessionUser();

                //Return all accounts associated with the customer in session
                var accounts = _engine.GetAccounts((int)custID);

                //Send available accounts to the view
                ViewBag.Accounts = new SelectList(accounts, "AccountNumber", "AccountType");

                return View();
            }
            catch (CustomDatabaseException e)
            {
                _logger.LogError($"Database: {e}");
            }
            catch (CustomTransactionException e)
            {
                _logger.LogInformation($"Transaction: {e}");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Route("New")]
        public async Task<IActionResult> Create(BillPay bill, char payPeriod)
        {
            try
            {
                var payee = await _context.Payees.FirstOrDefaultAsync(x => x.PayeeID == bill.PayeeID);

                if (payee == null)
                    ModelState.AddModelError("PayeeError", "Payee Number does not exist.");

                if (bill.ScheduleDate < DateTime.Now)
                    ModelState.AddModelError("TimeError", "Date must not be in the past.");

                if (payPeriod == 0)
                    ModelState.AddModelError("PeriodError", "A period must be selected.");

                if (bill.Amount == 0)
                    ModelState.AddModelError("AmountError", "Amount must be larger than 0");


                if (ModelState.ErrorCount > 2)

                {
                    var custID = GetsessionUser();

                    var accounts = _engine.GetAccounts((int)custID);
                    ViewBag.Accounts = new SelectList(accounts, "AccountNumber", "AccountType");

                    return View();
                }

                var response = await _engine.NewBillPay(bill.PayeeID, bill.AccountNumber,
                                                bill.Amount, bill.ScheduleDate, payPeriod);

                if (!response)
                    return RedirectToAction(nameof(Index));
            }
            catch (CustomDatabaseException e)
            {
                _logger.LogError($"Database: {e}");
            }
            catch (CustomTransactionException e)
            {
                _logger.LogInformation($"Transaction: {e}");
            }

            return RedirectToAction(nameof(Index));
        }


        private int? GetsessionUser()
        {
            return HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
        }
    }
}