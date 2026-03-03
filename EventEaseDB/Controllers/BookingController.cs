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
            var bookings = await _context.Booking.Include(b => b.Event).Include(b => b.Venue).ToListAsync();
            return View(bookings);
        }

        public IActionResult Create()
        {
            ViewData["Events"] = _context.Event.ToList();
            ViewData["Venues"] = _context.Venue.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Booking booking)
        {
            var eventDate = _context.Event.FirstOrDefault(e => e.EventId == booking.EventId)?.EventDate;

            var conflict = await _context.Booking.AnyAsync(b => b.VenueId == booking.VenueId && _context.Event.Any(e => e.EventId == b.EventId && e.EventDate == eventDate));
            if (conflict) {
                ModelState.AddModelError("", "This venue is already booked for that date.");
            }
            if (ModelState.IsValid)
            {
                _context.Booking.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Events"] = _context.Event.ToList();
            ViewData["Venues"] = _context.Venue.ToList();
            return View(booking);
        }
    }
}
