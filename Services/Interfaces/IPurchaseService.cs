
using SistemaDistribuidora.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Services.Interfaces;

public interface IPurchaseService
{
    Task CreateNewPurchase(PurchaseCreateDto purchase);

}
