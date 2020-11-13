using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WDT2020_a2.Models
{
    public enum AccountType
    {
        Checking = 'C',
        Saving = 'S'
    }

    public class Account
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Account Number")]
        public int AccountNumber { get; set; }

        [Required]
        [Display(Name = "Type")]
        public char AccountType { get; set; }

        [Required]
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [Required, DataType(DataType.Currency)]
        [Display(Name = "Account Balance")]
        public double Balance { get; set; }

        [Required, DataType(DataType.Date)]
        [Display(Name = "Date Modified")]
        public DateTime ModifyDate { get; set; }

        public virtual List<Transaction> Transactions { get; set; }

        public virtual List<BillPay> BillPays { get; set; }
    }
}
