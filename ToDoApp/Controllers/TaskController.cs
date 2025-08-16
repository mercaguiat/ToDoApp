using Microsoft.AspNetCore.Mvc;

namespace ToDoApp.Controllers
{
    public class TaskController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
