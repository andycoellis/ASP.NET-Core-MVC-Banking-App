using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WDT2020_a2.Attributes;
using WDT2020_a2.Data;
using WDT2020_a2.Services;
using WDT2020_a2.Models;
using WDT2020_a2.Services.Validation;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using WDT2020_a2.Exceptions;
using Microsoft.Extensions.Logging;

namespace WDT2020_a2.Controllers
{
    [AuthoriseCustomer]
    public class ProfileController : Controller
    {
        private readonly NwabContext _context;
        private readonly ILogger _logger;

        private readonly BankEngine _engine;

        public ProfileController(NwabContext context, ILogger<BillPayController> logger)
        {
            _context = context;
            _logger = logger;

            _engine = new BankEngine(_context);
        }


        public IActionResult Index()
        {
            var customer = GetSessionUser();
            Validator.ValidateUser(customer);

            return View(customer);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? customerID)
        {
            var customer = await _engine.GetCustomer(customerID);
            Validator.ValidateUser(customer);

            var states = _engine.GetStates().ToList();

            //Build a dropdown list of all states with assigned values
            ViewBag.Dropdown = (from c in states select new { c.State, c.Value }).Distinct();

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int customerID, string stateID, Customer customer)
        {
            if (customerID != customer.CustomerID)
                return NotFound();

            if (customer.Phone == null)
                ModelState.AddModelError("EditPhoneFailed", "Please enter a number");

            if (customer.Phone != null && customer.Phone.Length != 10)
                    ModelState.AddModelError("EditPhoneFailed", "Phone must be 10 digits long");

            if (customer.Phone != null && !customer.Phone.Substring(0, 1).Equals("0"))
                ModelState.AddModelError("EditPhoneFailed", "Please only input mobile phone numbers");

            if (customer.TFN != null && customer.TFN.Length < 9)
                ModelState.AddModelError("TfnFailed", "TFN numbers must be 9 digits in length");

            if (!ModelState.IsValid)
            {
                var cust = await _engine.GetCustomer(customerID);
                Validator.ValidateUser(cust);

                var states = _engine.GetStates().ToList();

                ViewBag.Dropdown = (from c in states select new { c.State, c.Value }).Distinct();

                return View(customer);
            }


            if (customer.Phone.Length == 10)
                customer.Phone = customer.Phone;

            if(stateID != null)
                customer.State = stateID;


            try
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();
            }
            catch(CustomDatabaseException e)
            {
                _logger.LogError(e.Message);
            }


            return RedirectToAction(nameof(Index));
        }

        private Customer GetSessionUser()
        {
            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));

            return _context.Customers.FirstOrDefault(x => x.CustomerID == customerID);
        }
    }
}
