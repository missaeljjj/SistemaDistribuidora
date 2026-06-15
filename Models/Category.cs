using SistemaDistribuidora.Exceptions;

namespace SistemaDistribuidora.Models;

public class Category
{
    private int _IdCategory;
    private string _CategoryName = "";

    public int IdCategory
    {
        get => _IdCategory;
        private set => _IdCategory = value;
    }

    public string Name
    {
        get => _CategoryName;
        private set => _CategoryName = !string.IsNullOrWhiteSpace(value) ? value.Trim() : throw new ValidationException("El nombre de la categoría no puede estar vacío" , nameof(Name));
    }

    public Category(int idcategory, string name)
    {
        this.IdCategory = idcategory;
        this.Name = name;
    }

}
