using Microsoft.AspNetCore.Mvc;

namespace QualificationSite.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
            
        }
        public IActionResult Index() => View();
    }
}