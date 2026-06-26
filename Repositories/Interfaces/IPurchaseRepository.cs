using SistemaDistribuidora.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Repositories.Interfaces;

public interface IPurchaseRepository 
{
    Task CreateNewPurchaseAsync(Purchase purchase);
}