    namespace SistemaDistribuidora.DTOs;

/// <summary>
/// DTO Para actualizar la informacion de un empleado
/// El DTO hereda de PersonUpdateDto, lo que permite reutilizar la estructura comun de los detalles de una persona
/// En este caso se agrega los otros campos de empleados por lo cual se agrega ID Y POSICION
/// <param name="EmployeeId"> Id del empleado para usar en la base de datos </param>
/// <param name="Position"> Cargo o posicion del empleado dentro de la empresa </param>"
/// </summary>
public class EmployeeUpdateDto : PersonUpdateDto
{
    public int EmployeeId { get; init; }
    public string? Position { get; init; } = "";
}
