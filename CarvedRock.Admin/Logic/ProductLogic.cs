using CarvedRock.Admin.Repository;
using CarvedRock.Admin.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using FluentValidation;

namespace CarvedRock.Admin.Logic;

public class ProductLogic : IProductLogic
{
    private readonly ICarvedRockRepository _repo;

    public IValidator<ProductModel> _validator { get; }

    public ProductLogic(ICarvedRockRepository repo, IValidator<ProductModel> validator)
    {
        _repo = repo;
        _validator = validator;
    }

    public async Task AddNewProduct(ProductModel productToAdd)
    {
        await _validator.ValidateAndThrowAsync(productToAdd);

        var productToSave = productToAdd.ToProduct();
        await _repo.AddProductAsync(productToSave);
    }

    public async Task<List<ProductModel>> GetAllProducts()
    {
        var products = await _repo.GetAllProductsAsync();

        // convert products from DB to product models
        // return products.Select(ProductModel.FromProduct).ToList();

        // the above is more terse syntax for: 
        var models = new List<ProductModel>();
        foreach (var product in products)
        {
            models.Add(ProductModel.FromProduct(product));
        }
        return models;
    }

    public async Task<ProductModel?> GetProductById(int id)
    {
        var product = await _repo.GetProductByIdAsync(id);
        return product == null ? null : ProductModel.FromProduct(product);
    }

    public async Task<ProductModel> InitializeProductModel()
    {
        return new ProductModel
        {
            AvailableCategories = await GetAvailableCategoriesFromDb()
        };
    }

    private async Task<List<SelectListItem>> GetAvailableCategoriesFromDb()
    {
        var cats = await _repo.GetAllCategoriesAsync();
        var returnList = new List<SelectListItem> { new SelectListItem("None", "") };
        var availCatList = cats.Select(item => new SelectListItem(item.Name, item.Id.ToString()));
        returnList.AddRange(availCatList);
        return returnList;
    }

    public async Task RemoveProduct(int id)
    {
        await _repo.RemoveProductAsync(id);
    }

    public async Task UpdateProduct(ProductModel productToUpdate)
    {
        var productToSave = productToUpdate.ToProduct();
        await _repo.UpdateProductAsync(productToSave);
    }

    public async Task GetAvailableCategories(ProductModel productModel)
    {
        productModel.AvailableCategories = await GetAvailableCategoriesFromDb();
    }
}