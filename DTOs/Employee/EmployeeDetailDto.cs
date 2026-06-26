using System;
namespace SistemaDistribuidora.DTOs;

// <summary>
// DTO para representar los detalles de un empleado, incluyendo su información personal estado y cargo
// El DTO hereda de PersonDetailDto, lo que permite reutilizar la estructura comun de los detalles de una persona
// y agrega el campo especifico "Cargo" para representar el puesto o rol del empleado dentro de la empresa
// </summary>
public record EmployeeDetailDto(
    int Id, string FullName, string Address, string Phone,string IdentityCard,string TypeofPerson, bool Status,
    DateTime RegisterDate,string Position,int quantityof
) : PersonDetailDto(Id, FullName, Address, Phone,IdentityCard,TypeofPerson ,Status,RegisterDate);

public record EmployeeListDto(
    int Id, string FullName, string Address, string Phone, string IdentityCard, string TypeofPerson, bool Status,
    DateTime RegisterDate, string Position
) : PersonDetailDto(Id, FullName, Address, Phone, IdentityCard, TypeofPerson, Status, RegisterDate);

