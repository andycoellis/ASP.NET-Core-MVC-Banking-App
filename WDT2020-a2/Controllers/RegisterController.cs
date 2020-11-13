using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WDT2020_a2.Data;
using WDT2020_a2.Models;
using WDT2020_a2.Services;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using WDT2020_a2.Exceptions;
using Microsoft.Extensions.Logging;

namespace WDT2020_a2.Controllers
{
    [Route("/Nwab/Signup")]
    public class RegisterController : Controller
    {
        private readonly NwabContext _context;
        private readonly ILogger _logger;
    
        private readonly BankEngine _engine;

        public RegisterController(NwabContext context, ILogger<RegisterController> logger)
        {
            _context = context;
            _logger = logger;
            _engine = new BankEngine(_context);
        }

        public IActionResult Index() => View("Register");

        [HttpPost]
        public async Task<IActionResult> Register(string customerName, string phone)
        {
            if (!Regex.IsMatch(phone, @"^[\d]{10}$"))
                ModelState.AddModelError("RegisterFailed", "Please only enter 10 numbers");

            if (customerName == null)
                ModelState.AddModelError("RegisterFailed", "Please enter your name");

            if (!ModelState.IsValid)
                return View();

            try
            {
                var customer = await _engine.NewCustomer(customerName, phone);

                //Set session from creation of a new user
                HttpContext.Session.SetInt32(nameof(Customer.CustomerID), customer.CustomerID);
                HttpContext.Session.SetString(nameof(Customer.CustomerName), customer.CustomerName);

                return View("Login");
            }
            catch(CustomDatabaseException e)
            {
                _logger.LogError(e.Message);
            }
            return View();

        }

        [Route("/Nwab/Register")]
        [HttpPost]
        public async Task<IActionResult> Login(string password, string checkPassword)
        {
            var custID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));

            if (custID == null)
                new RedirectToActionResult("Index", "Home", null);

            if (password == null)
                ModelState.AddModelError("RegisterFailed", "Please a password");

            if (!password.Equals(checkPassword))
                ModelState.AddModelError("RegisterFailed", "Passwords did not match");

            if (!ModelState.IsValid)
                return View();

            try
            {
                await _engine.NewLogin(password, (int)custID);

                return RedirectToAction("Index", "Accounts");
            }

            catch (CustomDatabaseException e)
            {
                _logger.LogError(e.Message);
            }

            return View(nameof(Index));
        }
    }
}
