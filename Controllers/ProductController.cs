using AzureRetailApp.Models;
using AzureRetailApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace AzureRetailApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductTableService _svc;

        public ProductController(IConfiguration config)
        {
            var conn = config.GetConnectionString("AzureStorageConnection");
            _svc = new ProductTableService(conn, "Products");
        }

        // GET /Product
        public async Task<IActionResult> Index()
        {
            var products = await _svc.GetAllAsync();
            return View(products); // Views/Product/Index.cshtml -> @model IEnumerable<ProductEntity>
        }

        // GET /Product/Create
        public IActionResult Create() => View(new ProductEntity());

        // POST /Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductEntity model)
        {
            // Required fields
            if (string.IsNullOrWhiteSpace(model.Name))
                ModelState.AddModelError(nameof(model.Name), "Name is required.");
            if (string.IsNullOrWhiteSpace(model.Category))
                ModelState.AddModelError(nameof(model.Category), "Category is required.");

            // Force-parse "price" from the form -> double (Table Storage type)
            var priceRaw = (Request.Form["price"].ToString() ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(priceRaw))
            {
                ModelState.AddModelError("Price", "Price is required.");
            }
            else
            {
                // Normalize: remove spaces; make decimal separator '.'
                priceRaw = priceRaw.Replace(" ", "").Replace(",", ".");
                if (!double.TryParse(priceRaw, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed))
                {
                    ModelState.AddModelError("Price", "Enter a valid number, e.g. 1999.99");
                }
                else
                {
                    model.Price = parsed; // ✅ explicit assignment
                }
            }

            if (!ModelState.IsValid)
                return View(model);

            // Keys
            model.PartitionKey = "Retail";
            model.RowKey = Guid.NewGuid().ToString("n");

            await _svc.AddAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // GET /Product/Delete?partitionKey=Retail&rowKey=...
        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            if (string.IsNullOrWhiteSpace(partitionKey) || string.IsNullOrWhiteSpace(rowKey))
                return BadRequest();

            await _svc.DeleteAsync(partitionKey, rowKey);
            return RedirectToAction(nameof(Index));
        }
    }
}
