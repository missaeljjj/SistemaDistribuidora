using System;


namespace SistemaDistribuidora.Exceptions;

public class EntityNotFoundException : DomainException
{
    public string EntityName { get; }   
    public int EntityId { get; }

    public EntityNotFoundException(string entityname,int id) 
        : base($"No se encontro {entityname} con Id {id}")
    {
        this.EntityName = entityname;
        this.EntityId = id;
    }

}
