using System;   
namespace SistemaDistribuidora.Exceptions;

public class ConnectionException : DomainException
{
    public ConnectionException(string message, Exception inner) : base(message, inner) { }
}
