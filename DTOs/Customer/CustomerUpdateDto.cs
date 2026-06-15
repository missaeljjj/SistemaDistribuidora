namespace SistemaDistribuidora.DTOs;
/// <summary>
/// DTO para actualizar la información de un cliente, heredando las propiedades de PersonUpdateDto.
/// </summary>
public class CustomerUpdateDto : PersonUpdateDto
{
    public int CustomerId { get; init; }
}