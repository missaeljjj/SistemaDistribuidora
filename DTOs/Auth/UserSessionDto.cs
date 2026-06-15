namespace SistemaDistribuidora.DTOs.Auth;

public record UserSessionDto
(
 int UserId,
 int EmployeeId,
 string Username,
 string Role
);

