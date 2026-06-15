using System.Collections.Generic;

namespace SistemaDistribuidora.DTOs;

///<<summary>
/// DTO base para la creación de personas, que puede ser heredado por
// DTOs específicos como ClienteCreateDto o ProveedorCreateDto
/// <param name = "FullName" > Nombre completo de la persona </param>
/// <param name="Identity">Número de identificacion o documento de la persona</param>
/// <param name="Address">Direccion de la persona</param>
/// <param name="PersonType">Tipo de persona (Juridico,Persona)</param>
/// <param name="Phones">Lista de telefonos asociados a la persona</param>
//<<summary>
public abstract class PersonCreateDto 
{
    public string FullName { get; init; } = "";
    public string Identity { get; init; } = "";
    public string Address { get; init; } = "";
    public string PersonType { get; init; } = "";
    public string Phone { get; init; } = "";
}



