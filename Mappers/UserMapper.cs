using SistemaDistribuidora.Models;
using SistemaDistribuidora.DTOs.Auth;

namespace SistemaDistribuidora.Mappers;

public static class UserMapper
{
    public static UserSessionDto ToUserDto(this User user)
        => new UserSessionDto
        (
            UserId: user.UserId,
            EmployeeId: user.EmployeeId,
            Username: user.Username,
            Role: user.Role
        );


}
