using SistemaDistribuidora.Models;
using SistemaDistribuidora.Repositories.DataBaseConnection;
using SistemaDistribuidora.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDistribuidora.Repositories.Implementation
{
    public class SupplierRepository : ISuplierRepository
    {
        private readonly IDataBase _DataBase;

        public SupplierRepository(IDataBase dataBase)
        {
            _DataBase = dataBase;
        }

        public async Task InsertAsync(Supplier supplier)
        {

        }

        public async Task UpdateAsync(Supplier supplier)
        {

        }

        public async Task DeleteAsync(int SupplierId)
        {

        }

        public async Task<Supplier> GetByIdAsync(int SupplierId)
        {
            return null!;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            return null!;
        }

        public Task<IEnumerable<Supplier>> GetAllSuppliersSummary()
        {
            return null!;
        }


    }
}
