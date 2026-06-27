using SistemaDistribuidora.Services.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using SistemaDistribuidora.DTOs;
using SistemaDistribuidora.Repositories.Interfaces;
using SistemaDistribuidora.Exceptions;
using SistemaDistribuidora.Mappers;


namespace SistemaDistribuidora.Services.Implementation;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _EmployeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _EmployeeRepository = employeeRepository;        
    }

    public async Task CreateNewEmployee(EmployeeCreateDto dto)
    {
        bool existing = await _EmployeeRepository.EmployeeExisting(dto.Identity);

        if(existing)
            throw new BussinessRulesException("Idenificacion duplicada",$"ya existe un empleado con identificacion: {dto.Identity}" );
        
        var customer = dto.ToModel();

        await _EmployeeRepository.InsertAsync(customer);

    }

    public async Task UpdateEmployee(EmployeeUpdateDto dto)
    {
        var existing = await _EmployeeRepository.GetByIdAsync(dto.EmployeeId);

        bool duplicated = await _EmployeeRepository.EmployeeExistingForUpdate(dto.Identity ?? existing.IdentityCard,dto.EmployeeId);

        if(duplicated)
            throw new BussinessRulesException("Idenificacion duplicada",$"ya existe un empleado con identificacion: {dto.Identity}" );

        var update = dto.ToModel(existing);

        await _EmployeeRepository.UpdateAsync(update);

    }

    public async Task DeleteEmployee(int EmployeeId)
    {
        await _EmployeeRepository.GetByIdAsync(EmployeeId);

        await _EmployeeRepository.DeleteAsync(EmployeeId);
    }

    public async Task<IEnumerable<EmployeeListDto>> GetAllEmployees(int EmployeeId)
    {
        //No implementation for this version
        return null!;
    }

    public async Task<IEnumerable<EmployeeDetailDto>> GetEmployeesDetail()
    {
        var EmployeeDetail = await _EmployeeRepository.GetAllEmployeesWithQuantityofSaleAsync();

        return EmployeeDetail.ToListDetail();

    }

}
