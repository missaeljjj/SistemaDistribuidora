namespace SistemaDistribuidora.DTOs;

/// <summary>
/// DTO Para actualizar informacion teniendo todas las propiedas necesarias 
/// lo cual solo agregamos el id para confirmar luego en la base de datos a quien se actualiza dicha informacion
/// </summary>

public class PersonUpdateDto 
{
    public int IdPerson { get; init; } 
    public string? FullName { get; init; } 
    public string? Identity { get; init; } 
    public string? Address { get; init; } 
    public string? Phone { get; init; } 
    public string? PersonType { get; init; } 
    public bool? Status { get; init; }


    
}

