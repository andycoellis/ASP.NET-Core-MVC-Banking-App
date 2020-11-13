using System;
namespace WDT2020_a2.Exceptions
{
    public class CustomTransactionException : Exception
    {
        public CustomTransactionException(string acctNumber, string message)
                : base($"ACCT NO: [{acctNumber}] Message: {message}")
        { }

        public CustomTransactionException(string acctNumber, string billPayID, string message)
        : base($"ACCT NO: [{acctNumber}] Message: {message}, for BILL: [{billPayID}]")
        { }


        public CustomTransactionException(string message)
                : base($"{message}")
        { }
    }
}
