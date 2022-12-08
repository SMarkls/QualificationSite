using Microsoft.AspNetCore.Mvc;
using QualificationSite.Services.Interfaces;

namespace QualificationSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDataBaseService db;

        public HomeController(IDataBaseService db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index()
        {
            var response = await db.GetProfilesAsync();
            var model = response.Data;
            return View(model);
        }
    }
}