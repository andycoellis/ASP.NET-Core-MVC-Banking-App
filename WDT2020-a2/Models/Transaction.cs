using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WDT2020_a2.Models
{
    public enum TransactionType
    {
        Deposit = 'D',
        Withdrawal = 'W',
        Transfer = 'T',
        ServiceCharge = 'S',
        BillPay = 'B'
    }

    public class Transaction
    {
        [Required, Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TransactionID { get; set; }

        [Required]
        [Display(Name = "Transaction Type")]
        public char TransactionType { get; set; }

        [Required]
        [Display(Name = "Account Number")]
        public int AccountNumber { get; set; }
        public virtual Account Account { get; set; }

        
        [Display(Name = "Destination Account")]
        public int DestAccount { get; set; }

        [DataType(DataType.Currency)]
        public double Amount { get; set; }

        [MaxLength(255)]
        public string Comment { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date Modified")]
        public DateTime ModifyDate { get; set; }
    }
}
