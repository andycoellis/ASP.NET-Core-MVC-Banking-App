using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WDT2020_a2.Data;
using WDT2020_a2.Exceptions;
using WDT2020_a2.Models;
using WDT2020_a2.Utilities;

namespace WDT2020_a2.Services.BankingServices
{
    public class CustomerService
    {
        private const int ID_LENGTH = 4;

        private readonly NwabContext _context;


        public CustomerService(NwabContext context)
        {
            _context = context;
        }

        /// <summary>Creates a new Customer object with auto Customer ID</summary>
        public async Task<Customer> CreateClient(string name, string phoneNumber)
        {
            string id = "";

            do
            {
                id = UtilityFunctions.GenerateStringID(ID_LENGTH);
            //Search if the new identification number already exists in the database
            } while (_context.Customers.FirstOrDefault(x => x.CustomerID == Convert.ToInt32(id)) != null);

            var customer = new Customer { CustomerID = Convert.ToInt32(id), CustomerName = name, Phone = phoneNumber };

            try
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                throw new CustomDatabaseException(nameof(Customer), e.Message);
            }

            return customer;
        }

        public async Task<Customer> GetClient(int? customerID)
        {
            return await _context.Customers.FirstOrDefaultAsync(x => x.CustomerID == customerID);
        }

        public bool DoesClientExist(int id)
        {
            return _context.Customers.Any(e => e.CustomerID == id);
        }

        public async Task<Customer> GetClient(int customerID)
        {
            return await _context.Customers.FirstOrDefaultAsync(x => x.CustomerID == customerID);
        }

        public async Task<Customer> GetCustomerWithAccounts(int customerID)
        {
            return await _context.Customers.Include(x => x.Accounts).FirstAsync(x => x.CustomerID == customerID);
        }

        public async Task UpdateCustomer(Customer customer)
        {
            try
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                throw new CustomDatabaseException(nameof(Customer), e.Message);
            }

        }
    }
}
