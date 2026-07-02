using SistemaDistribuidora.Repositories.Interfaces;
using SistemaDistribuidora.Repositories.DataBaseConnection;
using System.Threading.Tasks;
using SistemaDistribuidora.Models;
using System.Collections.Generic;
using SistemaDistribuidora.Exceptions;
using System;
using Dapper;
using System.Data;
using System.Linq;
using System.Data.Common;

namespace SistemaDistribuidora.Repositories.Implementation;


public class CategoryRepository : ICategoryRepository
{
    private readonly IDataBase _DataBase;

    public CategoryRepository(IDataBase database)
    {
        _DataBase = database;
    }

    public async Task InsertAsync(Category category)
    {
        DbConnection Connection;
        try
        {
            Connection = await _DataBase.GetConnectionAsync();
        }
        catch (Exception ex)
        {
            throw new ConnectionException("Error en conexion base de datos", ex);
        }
       
        try
        {
            await Connection.ExecuteAsync
                (
                   "sp_CreateNewCategory",
                   new
                   {
                       CategoryName = category.Name
                   },
                   commandType: CommandType.StoredProcedure
                );
        }
        catch(Exception e)
        {
            throw new DataBaseOperationException("sp_CreateNewCategory", "Error al insertar", e);
        }
    }

    public async Task UpdateAsync(Category category)
    {
        DbConnection Connection;
        try
        {
            Connection = await _DataBase.GetConnectionAsync();
        }
        catch (Exception ex)
        {
            throw new ConnectionException("Error en conexion base de datos", ex);
        }

        try
        {
            await Connection.ExecuteAsync
                (
                    "sp_UpdateCategory",
                    new { CategoryId = category.IdCategory, CategoryName = category.Name },
                    commandType: CommandType.StoredProcedure
                );
        }
        catch(Exception e)
        {
            throw new DataBaseOperationException("sp_UpdateCategory", "Error al actualizar", e);
        }
    }

    public async Task DeleteAsync(int CategoryId)
    {
        DbConnection Connection;
        try
        {
            Connection = await _DataBase.GetConnectionAsync();
        }
        catch (Exception ex)
        {
            throw new ConnectionException("Error en conexion base de datos", ex);
        }

        try
        {
            await Connection.ExecuteAsync
                (
                    "sp_DeleteCategory",
                    new { CategoryId = CategoryId},
                    commandType: CommandType.StoredProcedure
                );
        }
        catch (Exception e)
        {
            throw new DataBaseOperationException("sp_DeleteCategory", "Error al eliminar", e);
        }
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        DbConnection Connection;
        try
        {
            Connection = await _DataBase.GetConnectionAsync();
        }
        catch (Exception ex)
        {
            throw new ConnectionException("Error en conexion base de datos", ex);
        }

        try
        {
            const string sql = "SELECT * FROM vw_AllCategories";
            var Rows = await Connection.QueryAsync<CategoryMap>(sql);

            return Rows.Select(r => r.ToCategory());

        }
        catch (Exception e)
        {
            throw new DataBaseOperationException("SELECT * FROM vw_AllCategories", "Error al mostrar", e);
        }

    }

    public async Task<Category> GetByIdAsync(int CategoryId)
    {
        DbConnection Connection;
        try
        {
            Connection = await _DataBase.GetConnectionAsync();
        }
        catch (Exception ex)
        {
            throw new ConnectionException("Error en conexion base de datos", ex);
        }

        try
        {
            var row = await Connection.QuerySingleOrDefaultAsync<CategoryMap>
                        (
                           "sp_GetCategoryById",
                           new { CategoryId = CategoryId },
                           commandType: CommandType.StoredProcedure
                        );
            if (row == null)
                throw new EntityNotFoundException("Categoria",CategoryId);

            return row.ToCategory();

        }
        catch (Exception e)
        {
            throw new DataBaseOperationException("sp_GetCategoryById", "Error al encontrar categoria", e);
        }
    }

    public async Task<IEnumerable<(Category category, int quantityofproducts)>> GetAllCategoriesWithQuantityOfProductsAsync()
    {
        DbConnection Connection;
        try
        {
            Connection = await _DataBase.GetConnectionAsync();
        }
        catch (Exception ex)
        {
            throw new ConnectionException("Error en conexion base de datos", ex);
        }

        try
        {
            const string sql = "SELECT * FROM vw_CategoryWithQuantityOfProducts";

            var rows = await Connection.QueryAsync<CategoryDetailWithQuantityOfproducts>(sql);

            return rows.Select(r => r.ToTuple());
        }
        catch(Exception e)
        {
            throw new DataBaseOperationException("SELECT * FROM vw_CategoryWithQuantityOfProducts", "Error al obtener", e);

        }
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        DbConnection connection;
        try
        {
            connection = await _DataBase.GetConnectionAsync();
        }
        catch (Exception ex)
        {
            throw new ConnectionException("Error en conexion base de datos", ex);
        }

        try
        {

            const string sql = @"SELECT CASE WHEN EXISTS (SELECT 1 FROM Category WHERE LOWER(TRIM(Name)) = LOWER(TRIM(@Name)) ) THEN 1 ELSE 0 END";

            var result = await connection.ExecuteScalarAsync<int>(sql, new { Name = name });
            
            return result == 1;     
        }
        catch(Exception e)
        {
            throw new DataBaseOperationException("command","Error al obtener datos",e);            
        }

           
    }

    public async Task<bool> ExistsByNameExcludedAsync(string name, int idToExclude)
    {
        DbConnection connection;
        try
        {
            connection = await _DataBase.GetConnectionAsync();
        }
        catch (Exception ex)
        {
            throw new ConnectionException("Error en conexion base de datos", ex);
        }

        try
        {

            const string sql = @"SELECT CASE WHEN EXISTS (SELECT 1 FROM Category WHERE LOWER(TRIM(Name)) = LOWER(TRIM(@Name)) AND CategoryId <> @IdToExclude ) THEN 1 ELSE 0 END";

            var result = await connection.ExecuteScalarAsync<int>(sql, new { Name = name, IdToExclude = idToExclude });
        
            return result == 1;

        }
        catch(Exception e)
        {
            throw new DataBaseOperationException("command","Error al obtener datos",e);            
        }
    }
    
    #region MAPPERS PRIVADOS
    private class CategoryMap
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = "";

        public Category ToCategory() => new Category
            (
                idcategory: CategoryId,
                name: Name
            );
    }

    //Hereda para evitar repetir
    private class CategoryDetailWithQuantityOfproducts  : CategoryMap
    {
        public int QuantityOfProducts { get; set; }

        public (Category Category, int QuantityOfProducts) ToTuple() =>
            (ToCategory(), QuantityOfProducts);
    }
    #endregion

}





