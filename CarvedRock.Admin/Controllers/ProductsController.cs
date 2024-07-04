using System.Diagnostics;
using System.Threading.Tasks.Sources;
using CarvedRock.Admin.Logic;
using CarvedRock.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarvedRock.Admin.Controllers;

public class ProductsController : Controller
{
    private readonly IProductLogic _logic;

    public ILogger<ProductsController> _logger { get; }

    // public List<ProductModel> Products { get; set; }

    public ProductsController(IProductLogic logic, ILogger<ProductsController> logger)
    {
        // Products = GetSampleProducts();
        _logic = logic;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _logic.GetAllProducts();
        return View(products);
    }

    public async Task<IActionResult> Details(int id)
    {
        // var product = Products.Find(p => p.Id == Id);
        var product = await _logic.GetProductById(id);

        // return product is null ? View("NotFound") : View(product);
        if (product == null)
        {
            _logger.LogInformation("Details not found for id {id}", id);
            return View("NotFound");
        }

        return View(product);
    }

    public async Task<IActionResult> CreateAsync()
    {
        var model = await _logic.InitializeProductModel();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id, Name, Description, Price, IsActive, CategoryId")] ProductModel product)
    {
        if (!ModelState.IsValid) return View(product);

        await _logic.AddNewProduct(product);
        return RedirectToAction(nameof(Index));

    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            _logger.LogInformation("No id is passed for Edit");
            return View("NotFound");
        }

        var productModel = await _logic.GetProductById(id.Value);
        if (productModel == null)
        {
            _logger.LogInformation("Edit details not found for id {id}", id);
            return View("NotFound");
        }

        return View(productModel);

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id, Name, Description, Price, IsActive")] ProductModel product)
    {
        if (id != product.Id) return View("Not Found");

        if (!ModelState.IsValid) return View(product);

        await _logic.UpdateProduct(product);
        return RedirectToAction(nameof(Index));
    }

    // GET : Products/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return View("NotFound");

        var productModel = await _logic.GetProductById(id.Value);
        if (productModel == null) return View("Not Found");

        return View(productModel);
    }

    // POST : Products/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _logic.RemoveProduct(id);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }

    private List<ProductModel>? GetSampleProducts()
    {
        return new List<ProductModel>()
        {
            new ProductModel {Id = 1, Name = "Trailblazer", Price = 69.99M, IsActive = true,
                Description = "Great support in this high-top to take you to great heights and trails." },
            new ProductModel {Id = 2, Name = "Coastliner", Price = 49.99M, IsActive = true,
                Description = "Easy in and out with this lightweight but rugged shoe with great ventilation to get your around shores, beaches, and boats."},
            new ProductModel {Id = 3, Name = "Woodsman", Price = 64.99M, IsActive = true,
                Description = "All the insulation and support you need when wandering the rugged trails of the woods and backcountry." },
            new ProductModel {Id = 4, Name = "Basecamp", Price = 249.99M, IsActive = true,
                Description = "Great insulation and plenty of room for 2 in this spacious but highly-portable tent."},
        };
    }
}