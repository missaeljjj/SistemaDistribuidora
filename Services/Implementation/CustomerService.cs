using System.Collections.Generic;
using System.Threading.Tasks;
using SistemaDistribuidora.DTOs;
using SistemaDistribuidora.Exceptions;
using SistemaDistribuidora.Mappers;
using SistemaDistribuidora.Repositories.Interfaces;
using SistemaDistribuidora.Services.Interfaces;

namespace SistemaDistribuidora.Services.Implementation;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _CustomerRepository;

    public CustomerService(ICustomerRepository CustomerRepository)
    {
        _CustomerRepository = CustomerRepository;        
    }

    public async Task CreateNewCustomer(CustomerCreateDto dto)
    {
        var existing = await _CustomerRepository.CustomerExisting(dto.Identity);

        if (existing)
            throw new BussinessRulesException("Identificacion ya existente", "No puede agregar a un cliente existente");

        var customer = dto.ToModel();

        await _CustomerRepository.InsertAsync(customer);    
    }

    public async Task DeleteCustomer(int customerId)
    {
        await _CustomerRepository.GetByIdAsync(customerId);

        await _CustomerRepository.DeleteAsync(customerId);
    }

    public async Task UpdateCustomer(CustomerUpdateDto dto)
    {
        var existing = await _CustomerRepository.GetByIdAsync(dto.CustomerId);

         bool duplicated = await _CustomerRepository.CustomerExistingForUpdate(dto.Identity ??  existing.IdentityCard,dto.CustomerId); 

         if(duplicated)
            throw new BussinessRulesException("Identificacion duplicada","Ya existe un cliente con esa identificacion");

        var updated = dto.ToModel(existing);
        await _CustomerRepository.UpdateAsync(updated);
        
    }

    public async Task<IEnumerable<CustomerListDto>>GetAllCustomers()
    {
        var customers = await _CustomerRepository.GetAllAsync();

        return customers.ToListDtoList();  
    }

    public async Task<IEnumerable<CustomerDetailDto>> GetCustomerDetail()
    {
        var customersdetail = await _CustomerRepository.GetAllWithQuantityOfPurchases();

        return customersdetail.ToDetailList();
    }
}
