using System;
namespace SistemaDistribuidora.Exceptions;
public class DataBaseOperationException : DomainException
{
    public string OperationName { get; }

    public DataBaseOperationException(string operationName, string message, Exception inner)
        : base(message, inner)
    {
        this.OperationName = operationName;
    }
}
