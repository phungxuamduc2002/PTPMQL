
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
[HttpPost]


public IActionResult Index(Person ps)
{
    string str0utout = "Xin chào" + ps.PersonId + "_" + ps.FullName + "_" + ps.Addree;
    ViewBag.InfoPerson = str0utout;
    return View();
}

