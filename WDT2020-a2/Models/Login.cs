using System;
using System.ComponentModel.DataAnnotations;

namespace WDT2020_a2.Models
{
    public class Login
    {
        [Required, Key, StringLength(8, MinimumLength=8, ErrorMessage = "ID must be 8 digits long")]
        [RegularExpression(@"^\b[\dA-Za-z]+\b$", ErrorMessage = "Please use only letters and digits without any spaces")]
        [Display(Name = "User Login")]
        public string UserID { get; set; }

        [Required, RegularExpression(@"\b\d{4}\b", ErrorMessage = "Four digits only")]
        [Display(Name = "Customer ID")]
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [Required, RegularExpression(@"^[^\s]+$", ErrorMessage = "Please do not use spaces")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required, DataType(DataType.Date)]
        [Display(Name = "Date Modified")]
        public DateTime ModifyDate { get; set; }
    }
}
