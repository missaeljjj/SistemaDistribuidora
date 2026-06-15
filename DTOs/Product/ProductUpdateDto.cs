namespace SistemaDistribuidora.DTOs;
/// <summary>
/// Necesario el id porque al ser una modificacion en sql necesitamos saber especificamente que producto 
/// hay que modificar en concreto
/// </summary>
public class ProductUpdateDto 
{
    public int ProductId { get; init; }

    public string? Name { get; init; }

    public int? CategoryId { get; init; }

    public int? SupplierId { get; init; }

    public bool? Status { get; init; }

}
