using System.IO;
using System.Threading.Tasks;
using AzureRetailApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AzureRetailApp.Controllers
{
    public class FilesController : Controller
    {
        private readonly FileShareService _files;

        public FilesController(IConfiguration config)
        {
            var conn = config.GetConnectionString("AzureStorageConnection");
            _files = new FileShareService(conn, "contracts"); // requires real Azure Storage
        }

        // GET /Files
        public IActionResult Index()
        {
            var model = _files.List(); // IEnumerable<string>
            return View(model);
        }

        // POST /Files/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file is null || file.Length == 0) return RedirectToAction(nameof(Index));

            var safeName = Path.GetFileName(file.FileName);
            await using var s = file.OpenReadStream();
            await _files.UploadAsync(s, safeName, file.ContentType ?? "application/octet-stream");
            return RedirectToAction(nameof(Index));
        }

        // GET /Files/Download?name=mydoc.pdf
        [HttpGet]
        public async Task<IActionResult> Download(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest("Missing name.");
            var safeName = Path.GetFileName(name);
            var stream = await _files.DownloadAsync(safeName);
            return File(stream, "application/octet-stream", safeName);
        }
    }
}
