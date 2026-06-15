using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaDistribuidora.DTOs;
/// <summary>
/// DTOs para para la creacion de productos
/// </summary>
public class ProductCreateDto
{
    public string Name { get; init; } = "";

    public int CategoryId { get; init; }

    public int SupplierId { get; init; }

}

