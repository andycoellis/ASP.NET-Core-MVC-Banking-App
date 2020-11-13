using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WDT2020_a2.Models
{
    public class Payee
    {
        private const string PHONE_NUMBER_FORMAT = "{0:(61)- #### ####}";


        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PayeeID { get; set; }

        [Required, MaxLength(50), RegularExpression(@"\b[A-Za-z\s]+\b+")]
        [Display(Name = "Payee Name")]
        public string PayeeName { get; set; }

        [MaxLength(50), RegularExpression(@"^\b[\dA-Za-z\s]\b+$")]
        public string Address { get; set; }

        [MaxLength(40), RegularExpression(@"^\b[\dA-Za-z\s]+$")]
        public string City { get; set; }

        [MaxLength(20)]
        [RegularExpression(@"(?i)\b(vic|qld|sa|act|nsw|wa|tas|nt|)\b")]
        public string State { get; set; }

        [RegularExpression(@"\b\d{4}\b")]
        public string PostCode { get; set; }

        private string _phone;

        [Required, MaxLength(15), RegularExpression(@"^[\d\s]+$")]
        public string Phone
        {
            get => _phone;

            set
            {
                _phone = string.Format(PHONE_NUMBER_FORMAT, Convert.ToInt32(value));
            }
        }

        public virtual List<BillPay> BillPays { get; set; }
    }
}