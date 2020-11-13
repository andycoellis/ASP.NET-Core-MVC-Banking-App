using System;
namespace WDT2020_a2.Exceptions
{
    public class CustomDatabaseException : Exception
    {
        public CustomDatabaseException(string type, string message)
                : base($"[{type}] Message: {message}")
        { }


        public CustomDatabaseException(string message)
                : base($"{message}")
        { }
    }
}
