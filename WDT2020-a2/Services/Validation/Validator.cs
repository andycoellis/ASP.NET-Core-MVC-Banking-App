using System;
using SimpleHashing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WDT2020_a2.Data;
using WDT2020_a2.Services;
using WDT2020_a2.Models;
using Microsoft.AspNetCore.Http;

namespace WDT2020_a2.Services.Validation
{
    public static class Validator
    {

        /*  Customer Validation
         *-----------------------------------------------------------*/
        public static void ValidateUser(Customer customer)
        {
            if (customer == null)
                new RedirectToActionResult("Index", "Home", null);
        }


        /*  Login Validation
         *-----------------------------------------------------------*/

        public static void ValidateLogin(Login login)
        {
            if (login == null)
                new RedirectToActionResult("Index", "Home", null);
        }
    }
}
