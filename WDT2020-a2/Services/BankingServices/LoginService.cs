using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleHashing;
using WDT2020_a2.Data;
using WDT2020_a2.Exceptions;
using WDT2020_a2.Models;
using WDT2020_a2.Utilities;

namespace WDT2020_a2.Services.BankingServices
{
    public class LoginService
    {
        private const int ID_LENGTH = 8;

        private readonly NwabContext _context;


        public LoginService(NwabContext context)
        {
            _context = context;
        }

        ///<summary>Returns a newly generated Login with a given password</summary>
        public async Task CreateClient(string password, int customerID)
        {
            string id = "";

            do
            {
                id = UtilityFunctions.GenerateStringID(ID_LENGTH);

            } while (_context.Logins.FirstOrDefault(x => x.UserID.Equals(id)) != null);

            var login = new Login { UserID = id, Password = PBKDF2.Hash(password), CustomerID = customerID, ModifyDate = DateTime.UtcNow };

            try
            {
                _context.Add(login);

                await _context.SaveChangesAsync();

            }
            catch(Exception e)
            {
                throw new CustomDatabaseException(nameof(Login), e.Message);
            }
        }

        public bool DoesClientExist(int id)
        {
            return _context.Logins.Any(e => Convert.ToInt32(e.UserID) == id);
        }
    }
}
