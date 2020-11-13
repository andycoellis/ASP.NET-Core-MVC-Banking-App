    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace WDT2020_a2.Models
{
    public class Customer
    {
        private const string PHONE_NUMBER_FORMAT = "{0:(61)- ### ### ###}";

        //Unique ID Required
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CustomerID { get; set; }

        [Required, MaxLength(50), RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Only letters of the alphabet")]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        //Identification Purposes Only
        [StringLength(9), RegularExpression(@"^[\d]+$", ErrorMessage = "An Individual TFN can only be 9 digits long")]
        [Display(Name = "Tax File Number")]
        public string TFN { get; set; }

        [MaxLength(50), RegularExpression(@"^\b[\dA-Za-z\s\-\\]+\b$", ErrorMessage = "Only letters and numbers")]
        public string Address { get; set; }

        [MaxLength(40), RegularExpression(@"^\b[A-Za-z\s]+$", ErrorMessage = "Please only use names.")]
        public string City { get; set; }

        //Must be 3 lettered Australian state, WA, NT???
        [MaxLength(3)]
        [RegularExpression(@"(?i)\b(vic|qld|sa|act|nsw|wa|tas|nt|)\b", ErrorMessage = "Abbreviated names only")]
        public string State { get; set; }

        //Must be a 4 digit number
        [RegularExpression(@"\b\d{4}\b", ErrorMessage = "Four digits only")]
        public string PostCode { get; set; }

        ////Must be in the format (61)- xxxx xxxx
        //[Required, StringLength(18, MinimumLength = 10, ErrorMessage = "Phone Number must be inputted with 10 digits")]
        //public string Phone { get; set; }

        private string _phone;

        [RegularExpression(@"^0{1}[\d]{9}$", ErrorMessage = "Must be an Australian Mobile number")]
        public string Phone
        {
            get
            {
                return $"0{_phone = Regex.Replace(_phone, @"(\(61\)|\-|\s)", "")}";
            }
            set
            {
                _phone = string.Format(PHONE_NUMBER_FORMAT, Convert.ToInt32(value));
            }
        }

        [NotMapped]
        public string DisplayPhoneNumber => _phone;

        //Link to accounts
        public virtual List<Account> Accounts { get; set; }
    }
}
