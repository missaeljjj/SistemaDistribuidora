using System;

namespace SistemaDistribuidora.Exceptions;

public class ValidationException : DomainException
{
     public string FieldName  { get; }
    public ValidationException(string message,string fieldName) : base(message) 
    {
        this.FieldName = fieldName;
    }
}
