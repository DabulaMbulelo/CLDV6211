using EventEaseDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventEaseDB.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDBContext _context;
        public BookingController(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Booking.ToListAsync();
            return View(bookings);
        }
    }
}
