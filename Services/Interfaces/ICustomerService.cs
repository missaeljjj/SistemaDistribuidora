using SistemaDistribuidora.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Services.Interfaces;

public interface ICustomerService
{
    Task CreateNewCustomer(CustomerCreateDto customercreatedto);

    Task UpdateCustomer(CustomerUpdateDto customerupdatedto);

    Task DeleteCustomer(int CustomerId);

    Task<IEnumerable<CustomerListDto>>GetAllCustomers(int CustomerId);

    Task <IEnumerable<CustomerDetailDto>> GetCustomerDetail();

}
