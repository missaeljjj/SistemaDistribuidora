using SistemaDistribuidora.Models;
using SistemaDistribuidora.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace SistemaDistribuidora.Mappers;

public static class CustomerMapper
{
    //Mapeo para pasar de DTO a model
    public static Customer ToModel(this CustomerCreateDto dto)
        => new Customer
        (
            idperson: 0,    //ASIGNADO POR LA BASE DE DATOS
            fullname: dto.FullName,
            typeofperson: dto.PersonType,
            identitycard: dto.Identity,
            address: dto.Address,
            phone: dto.Phone,
            registerdate: System.DateTime.Now,
            status: true,
            idcustomer: 0 //ASIGNADO POR LA BASE DE DATOS
        );

    public static Customer ToModel(this CustomerUpdateDto dto, Customer existing)
        => new Customer
        (
            idperson: dto.IdPerson,
            fullname: dto.FullName ?? existing.FullName,
            typeofperson: dto.PersonType ?? existing.TypeOfPerson,
            identitycard: dto.Identity ?? existing.IdentityCard,
            address: dto.Address ?? existing.Address,
            phone: dto.Phone ?? existing.Phone,
            registerdate: existing.RegisterDate,
            status: dto.Status ?? existing.Status,
            idcustomer: dto.CustomerId
        );

    //SUMMARY QUE NECESITAMOS PARA EL "CATALOGO" A SELECCIONAR DE CLIENTES
    //Utilizamos un metodo propio LINQ para asignar cada customer a summaryDto
    public static IEnumerable<CustomerSummaryDto> ToSummaryDtoList(this IEnumerable<Customer> customers)
        => customers.Select(c => c.ToSummaryDto());

    public static CustomerSummaryDto ToSummaryDto(this Customer customer)
        => new CustomerSummaryDto
        (
            CustomerId: customer.IdCustomer,
            CustomerName: customer.FullName
        );


    public static IEnumerable<CustomerListDto> ToListDtoList(this IEnumerable<Customer> customers)
        => customers.Select(c => c.ToListDto());

    private static CustomerListDto ToListDto(this Customer customer)
        => new CustomerListDto
        (
           Id: customer.IdCustomer,
           FullName: customer.FullName,
           TypeOfPerson: customer.TypeOfPerson,
           IdentityCard: customer.IdentityCard,
           Address: customer.Address,
           Phone: customer.Phone,
           RegisterDate: customer.RegisterDate,
           Status: customer.Status
        );

    public static IEnumerable<CustomerDetailDto> ToDetailList(this IEnumerable<(Customer customer, int quantityofpurchases)> customers)
        => customers.Select(c => c.customer.ToDetail(c.quantityofpurchases));

    private static CustomerDetailDto ToDetail(this Customer customer, int quantityofpurchases)
        => new CustomerDetailDto
        (
           Id: customer.IdCustomer,
           FullName: customer.FullName,
           TypeOfPerson: customer.TypeOfPerson,
           IdentityCard: customer.IdentityCard,
           Address: customer.Address,
           Phone: customer.Phone,
           RegisterDate: customer.RegisterDate,
           Status: customer.Status,
           QuantityOfPurchases: quantityofpurchases
        );








}
