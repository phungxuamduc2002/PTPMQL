
using Microsoft.AspNetCore.Mvc;

namespace MvcMovie.Controllers
{

    public class PersonController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
