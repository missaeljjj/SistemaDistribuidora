namespace SistemaDistribuidora.DTOs;

public class EmployeeCreateDto : PersonCreateDto
{
    public string Position { get; init; } = "";
}
