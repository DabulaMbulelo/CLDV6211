using EventEaseDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventEaseDB.Controllers
{
    public class VenueController : Controller
    {
        private readonly ApplicationDBContext _context;
        public VenueController(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var Venues = await _context.Venue.ToListAsync();
            return View(Venues);
        }
    }
}
