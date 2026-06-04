using System;

namespace SistemaDistribuidora.Models;

class Category
{
    private int CategoryId;
    private string CategoryName = "";

    public int IdCategory
    {
        get => CategoryId;
        private set => CategoryId = value;
    }

    public string Name
    {
        get => CategoryName;
        private set => CategoryName = !string.IsNullOrWhiteSpace(value) ? value.Trim() : throw new ArgumentException("El nombre de la categoría no puede estar vacío");
    }

    public Category(int idCategory, string name)
    {
        this.IdCategory = idCategory;
        this.Name = name;
    }

}
