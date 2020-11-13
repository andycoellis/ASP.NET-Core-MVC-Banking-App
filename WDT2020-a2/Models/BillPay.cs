using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WDT2020_a2.Attributes;

namespace WDT2020_a2.Models
{

    public enum PeriodLength
    {
        M = 1,
        Q = 3,
        A = 12,
        S = -1
    }

    public class BillPay
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required, Key]
        [Display(Name = "BillPay ID")]
        public int BillPayID { get; set; }

        [Required]
        [Display(Name = "Account Number")]
        public int AccountNumber { get; set; }
        public virtual Account Account { get; set; }

        [Required]
        [Display(Name = "Payee ID")]
        [RegularExpression(@"^[\d]{4}$", ErrorMessage = "Payee ID's are 4 digits long")]
        public int PayeeID { get; set; }
        public virtual Payee Payee { get; set; }

        [Required, DataType(DataType.Currency)]
        [RegularExpression(@"^(\d+(\.\d{0,2})?|\.?\d{1,2})$", ErrorMessage = "Amount must have more than two decimal places")]
        public double Amount { get; set; }

        private DateTime _scheduleDate;

        [Required]
        [Display(Name = "Date Scheduled")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy - hh:mm tt}")]
        public DateTime ScheduleDate
        {
            get => _scheduleDate.ToLocalTime();

            set => _scheduleDate = value.ToUniversalTime();
        }

        [Required]
        [Display(Name = "Frequency Period")]
        public char Period { get; set; }

        private DateTime _modifyDate;

        [Required]
        [Display(Name = "Date Modified ID")]
        public DateTime ModifyDate
        {
            get => _modifyDate.ToLocalTime();

            set => _modifyDate = value.ToUniversalTime();
        }

        public string DisplayPeriod()
        {
            string label;

            switch(Period)
            {
                case 'S':
                    label = "Single";
                    break;
                case 'M':
                    label = "Monthly";
                    break;
                case 'Q':
                    label = "Quaterly";
                    break;
                case 'A':
                    label = "Annually";
                    break;
                default:
                    label = "Undefined";
                    break;
            }

            return label;
        }
    }
}
