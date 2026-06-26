using SistemaDistribuidora.DTOs;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Services.Interfaces;

public interface ISaleService
{
    Task CreateNewSale(SaleCreateDto dto);
}
