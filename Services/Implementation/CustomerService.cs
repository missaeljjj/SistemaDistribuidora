using System.Collections.Generic;
using System.Linq;
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
    private readonly IAppCache _cache;

    public CustomerService(ICustomerRepository CustomerRepository, IAppCache cache)
    {
        _CustomerRepository = CustomerRepository;
        _cache = cache;
    }

    public async Task CreateNewCustomer(CustomerCreateDto dto)
    {
        var customer = dto.ToModel();

        await _CustomerRepository.InsertAsync(customer);
        await _cache.ReloadCustomerAsync();
    }

    public async Task DeleteCustomer(int customerId)
    {
        await _CustomerRepository.GetByIdAsync(customerId);
        await _CustomerRepository.DeleteAsync(customerId);
        await _cache.ReloadCustomerAsync();
    }

    public async Task UpdateCustomer(CustomerUpdateDto dto)
    {
        var existing = await _CustomerRepository.GetByIdAsync(dto.CustomerId);

        bool duplicated = await _CustomerRepository.CustomerExistingForUpdate(dto.Identity ?? existing.IdentityCard, dto.CustomerId);

        if (duplicated)
            throw new BussinessRulesException("Identificacion duplicada", "Ya existe un cliente con esa identificacion");

        var updated = dto.ToModel(existing);
        await _CustomerRepository.UpdateAsync(updated);
        await _cache.ReloadCustomerAsync();
    }

    public async Task<IEnumerable<CustomerListDto>> GetAllCustomers()
    {

        if (_cache.Customers == null || !_cache.Customers.Any())
        {
            await _cache.ReloadCustomerAsync();
        }

        return _cache.Customers ?? new List<CustomerListDto>();
    }

    public async Task<IEnumerable<CustomerDetailDto>> GetCustomerDetail()
    {
        var customersdetail = await _CustomerRepository.GetAllWithQuantityOfPurchases();
        return customersdetail.ToDetailList();
    }
}