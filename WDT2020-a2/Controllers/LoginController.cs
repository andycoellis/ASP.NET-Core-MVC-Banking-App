using System;
using SimpleHashing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WDT2020_a2.Data;
using WDT2020_a2.Services;
using WDT2020_a2.Models;
using Microsoft.AspNetCore.Http;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WDT2020_a2.Controllers
{
    [Route("Nwab/Login")]
    public class LoginController : Controller
    {
        private readonly NwabContext _context;

        private readonly BankEngine _engine;

        public LoginController(NwabContext context)
        {
            _context = context;
            _engine = new BankEngine(_context);
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string userID, string password)
        {
            //Check if a login of given number exists in the database
            var login = await _context.Logins.FirstOrDefaultAsync(x => x.UserID.Equals(userID));

            if (login == null || !PBKDF2.Verify(login.Password, password))
                ModelState.AddModelError("LoginFailed", "Unsuccessful login attempt");

            if (!ModelState.IsValid)
                return View();

            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.CustomerID == login.CustomerID);

            //Set session customer
            HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
            HttpContext.Session.SetString(nameof(Customer.CustomerName), customer.CustomerName);

            return RedirectToAction("Index", "Accounts");
        }

        [Route("Logout")]
        public IActionResult Logout()
        {
            // Logout customer.
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}
