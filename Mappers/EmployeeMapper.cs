using SistemaDistribuidora.DTOs;
using SistemaDistribuidora.Models;

namespace SistemaDistribuidora.Mappers;

public static class EmployeeMapper
{
    //QuantityOf debido que las vistas que utilizo con cantidad de ventas o compras
    //lo cual reautilizaremos esto
    public static EmployeeDetailDto ToDetailDto(this Employee employee, int QuantityOf)
        => new EmployeeDetailDto
        (
            Id: employee.Id,
            FullName: employee.FullName,
            Address: employee.Address,
            Phone: employee.Phone,
            IdentityCard: employee.IdentityCard,
            TypeofPerson: employee.TypeOfPerson,
            Status: employee.Status,
            RegisterDate: employee.RegisterDate,
            Position: employee.Position,
            quantityof: QuantityOf

        );

    // EmployeeCreateDto -> Employee
    public static Employee ToModel(this EmployeeCreateDto dto)
        => new Employee
        (
            idperson: 0,
            fullname: dto.FullName,
            typeofperson: dto.PersonType,
            identitycard: dto.Identity,
            address: dto.Address,
            phone: dto.Phone,
            registerdate: System.DateTime.Now,
            status: true,
            employeeposition: dto.Position,
            idemployee: 0

        );

    //EmployeeUpdateDto -> Employee
    public static Employee ToModel(this EmployeeUpdateDto dto, Employee existing)
        => new Employee
        (
            idperson: dto.IdPerson,
            fullname: dto.FullName ?? existing.FullName,
            typeofperson: dto.PersonType ?? existing.TypeOfPerson,
            identitycard: dto.Identity ?? existing.IdentityCard,
            address: dto.Address ?? existing.Address,
            phone: dto.Phone ?? existing.Phone,
            registerdate: existing.RegisterDate,
            status: dto.Status ?? existing.Status,
            employeeposition: dto.Position ?? existing.Position,
            idemployee: dto.EmployeeId

        );
}
