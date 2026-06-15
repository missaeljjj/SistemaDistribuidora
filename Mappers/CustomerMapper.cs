using SistemaDistribuidora.Models;
using SistemaDistribuidora.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace SistemaDistribuidora.Mappers;

public static class CustomerMapper
{

    //Pasar a CustomerDetail tenemos un campo calculado que proviene de count de la base de datos
    //por lo cual se pone como parametro para asignar
    public static CustomerDetailDto ToDetailDto(this Customer customer, int quantityofpurchases = 0 )
        => new CustomerDetailDto
        (
            Id:                 customer.IdCustomer,
            FullName:           customer.FullName,
            Address:            customer.Address,
            Phone :             customer.Phone,
            IdentityCard:       customer.IdentityCard,
            Status:             customer.Status,
            RegisterDate:       customer.RegisterDate,
            QuantityOfPurchases: quantityofpurchases


        );

    //Mapeo para pasar de DTO a model
    public static Customer ToModel(this CustomerCreateDto dto)
        => new Customer
        (
            idperson:     0,    //ASIGNADO POR LA BASE DE DATOS
            fullname:     dto.FullName,
            typeofperson: dto.PersonType,
            identitycard: dto.Identity,
            address:      dto.Address,
            phone:        dto.Phone,
            registerdate: System.DateTime.Now,
            status:       true,
            idcustomer:   0 //ASIGNADO POR LA BASE DE DATOS
        );

    public static Customer ToModel(this CustomerUpdateDto dto, Customer existing)
        => new Customer
        (
            idperson:     dto.IdPerson,
            fullname:     dto.FullName ?? existing.FullName,
            typeofperson: dto.PersonType ?? existing.TypeOfPerson,
            identitycard: dto.Identity ?? existing.IdentityCard,
            address:      dto.Address ?? existing.Address,
            phone:        dto.Phone ?? existing.Phone,
            registerdate: existing.RegisterDate,
            status:       dto.Status ?? existing.Status,
            idcustomer:   dto.CustomerId
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









}
