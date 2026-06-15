using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaDistribuidora.DTOs;

/// <summary>
/// DTO Para presentar los detalles de una persona
/// el cual se utilizara mas adelante en otros detalles de otras entidades
/// </summary>
/// <param name="Id"></param>
/// <param name="FullName"></param>
/// <param name="Address"></param>
/// <param name="Phone"></param>
/// <param name="Status"></param>

public abstract record PersonDetailDto
(
    int Id,
    string FullName,
    string Address,
    string Phone,
    string IdentityCard,
    bool Status,
    DateTime RegisterDate
);


