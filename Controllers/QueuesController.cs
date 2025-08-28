using System;
using System.Threading.Tasks;
using AzureRetailApp.Models;
using AzureRetailApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AzureRetailApp.Controllers
{
    public class QueuesController : Controller
    {
        private readonly QueueStorageService _queue;

        public QueuesController(IConfiguration config)
        {
            var conn = config.GetConnectionString("AzureStorageConnection");
            _queue = new QueueStorageService(conn, "retail-events");
        }

        // GET /Queues
        public async Task<IActionResult> Index()
        {
            var msgs = await _queue.PeekAsync(32);
            return View(msgs);
        }

        // POST /Queues/Enqueue
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enqueue(string type, string id, string description)
        {
            try
            {
                var evt = new QueuesEvent(
                    string.IsNullOrWhiteSpace(type) ? "Custom" : type,
                    string.IsNullOrWhiteSpace(id) ? Guid.NewGuid().ToString("n") : id,
                    description ?? string.Empty,
                    DateTimeOffset.UtcNow);

                await _queue.EnqueueAsync(evt);
                TempData["Message"] = "Message enqueued.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to enqueue: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
