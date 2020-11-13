using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WDT2020_a2.Models;

namespace WDT2020_a2.Attributes
{
    /*
     *  Original approach to this concept was first learnt from Matther Bolger
     *  in NwbaExample with Login Day 07 Tutorial
     */

    public class AuthoriseCustomerAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var customerID = context.HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            if (!customerID.HasValue)
                context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}
