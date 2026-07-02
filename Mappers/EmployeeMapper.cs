using SistemaDistribuidora.DTOs;
using SistemaDistribuidora.Models;
using System.Collections.Generic;
using System.Linq;

namespace SistemaDistribuidora.Mappers;

public static class EmployeeMapper
{
    //QuantityOf debido que las vistas que utilizo con cantidad de ventas 
    private static EmployeeDetailDto ToDetailDto(this Employee employee, int QuantityOf)
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

    public static IEnumerable<EmployeeDetailDto> ToListDetail(this IEnumerable<(Employee employee, int quantityOf)> employees)
        => employees.Select(e => e.employee.ToDetailDto(e.quantityOf));


    private static EmployeeListDto ToListDto(this Employee employee)
    => new EmployeeListDto
    (
        Id: employee.IdEmployee,
        FullName: employee.FullName,
        Address: employee.Address,
        Phone: employee.Phone,
        IdentityCard: employee.IdentityCard,
        TypeofPerson: employee.TypeOfPerson,
        Status: employee.Status,
        RegisterDate: employee.RegisterDate,
        Position: employee.Position
    );

    public static IEnumerable<EmployeeListDto> EmployeeTolistDto(this IEnumerable<Employee> employees)
    => employees.Select(e => e.ToListDto());

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
