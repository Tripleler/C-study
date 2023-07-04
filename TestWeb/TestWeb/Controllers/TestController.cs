using Microsoft.AspNetCore.Mvc;

namespace TestWeb.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
