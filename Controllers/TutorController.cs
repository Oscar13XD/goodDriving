using GoodDriving.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodDriving.Controllers
{
    [Authorize(Roles = "TUTOR")]
    public class TutorController : Controller
    {
        private readonly goodDrivingContext _context;

        public TutorController(goodDrivingContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

    }
}
