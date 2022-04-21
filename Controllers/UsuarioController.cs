using GoodDriving.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace GoodDriving.Controllers
{
    [Authorize(Roles = "USUARIO")]
    public class UsuarioController : Controller
    {
        private readonly goodDrivingContext _context;

        public UsuarioController(goodDrivingContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
