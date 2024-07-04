using CarvedRock.Admin.Models;

namespace CarvedRock.Admin.Logic;

public interface IProductLogic
{
    Task<List<ProductModel>> GetAllProducts();
    Task<ProductModel?> GetProductById(int id);
    Task AddNewProduct(ProductModel productToAdd);
    Task UpdateProduct(ProductModel productToUpdate);
    Task RemoveProduct(int id);
    Task<ProductModel> InitializeProductModel();
    Task GetAvailableCategories(ProductModel productModel);
}