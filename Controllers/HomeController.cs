using Microsoft.AspNetCore.Mvc;

namespace AzureRetailApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
