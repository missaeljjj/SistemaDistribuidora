using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaDistribuidora.DTOs;

public class CategoryUpdateDto : CategoryCreateDto
{
    public int CategoryId { get; init; }
}
