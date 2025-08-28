using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using AzureRetailApp.Models;
using AzureRetailApp.Services;
using System;
using System.Threading.Tasks;

namespace AzureRetailApp.Controllers
{
    public class TableController : Controller
    {
        private readonly TableStorageService _tableService;

        public TableController(IConfiguration config)
        {
            var conn = config.GetConnectionString("AzureStorageConnection");
            // Pass connection string + table name
            _tableService = new TableStorageService(conn, "Customers");
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _tableService.GetCustomersAsync();
            return View(customers);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerEntity customer)
        {
            if (!ModelState.IsValid) return View(customer);

            customer.PartitionKey = "Retail";
            customer.RowKey = Guid.NewGuid().ToString("n");

            await _tableService.AddCustomerAsync(customer);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            await _tableService.DeleteCustomerAsync(partitionKey, rowKey);
            return RedirectToAction(nameof(Index));
        }
    }
}
