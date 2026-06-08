using System;
namespace SistemaDistribuidora.Exceptions;
public class BussinessRulesException : DomainException
{
        public string RuleName { get; }
        public BussinessRulesException(string rulename, string message) 
            : base(message)
        {
            this.RuleName= rulename;
        }
}
