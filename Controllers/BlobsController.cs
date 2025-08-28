using System;
using System.Threading.Tasks;
using AzureRetailApp.Models;
using AzureRetailApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AzureRetailApp.Controllers
{
    public class BlobsController : Controller
    {
        private readonly BlobStorageService _blobs;
        private readonly QueueStorageService _queue;

        public BlobsController(IConfiguration config)
        {
            var conn = config.GetConnectionString("AzureStorageConnection");
            _blobs = new BlobStorageService(conn, "product-images");
            _queue = new QueueStorageService(conn, "retail-events");
        }

        // GET /Blobs
        public IActionResult Index() => View(_blobs.List());

        // POST /Blobs/Upload
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                await using var s = file.OpenReadStream();
                await _blobs.UploadAsync(s, file.FileName, file.ContentType ?? "application/octet-stream");

                await _queue.EnqueueAsync(new QueuesEvent(
                    "ImageUploaded",
                    Guid.NewGuid().ToString("n"),
                    $"Image uploaded: {file.FileName}",
                    DateTimeOffset.UtcNow));
            }
            return RedirectToAction(nameof(Index));
        }

        // GET /Blobs/Download?name=foo.jpg
        [HttpGet]
        public async Task<FileStreamResult> Download(string name)
        {
            var stream = await _blobs.DownloadAsync(name);
            return File(stream, "application/octet-stream", name);
        }
    }
}
