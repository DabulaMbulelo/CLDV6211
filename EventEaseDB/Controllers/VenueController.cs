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
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue)
        {
            if (ModelState.IsValid)
            {
                _context.Venue.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venue
                .FirstOrDefaultAsync(m => m.VenueId == id);

            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venue.FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null) return NotFound();

            var hasBookings = await _context.Booking.AnyAsync(b => b.VenueId == id);
            ViewData["IsBooked"] = hasBookings;

            return View(venue);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var venue = await _context.Venue.FindAsync(id);
            if (venue == null) return NotFound();

            var hasBookings = await _context.Booking.AnyAsync(b => b.VenueId == id);
            if (hasBookings)
            {
                ModelState.AddModelError("", "Cannot delete this venue because it has associated bookings.");
                var reloaded = await _context.Venue.FirstOrDefaultAsync(m => m.VenueId == id);
                ViewData["IsBooked"] = true;
                return View("Delete", reloaded ?? venue);
            }

            _context.Venue.Remove(venue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public bool VenueExists(int id)
        {
            return _context.Venue.Any(e => e.VenueId == id);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var venue = await _context.Venue.FindAsync(id);
            if (venue == null) return NotFound();
            return View(venue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venue venue)
        {
            if (id != venue.VenueId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }
    }
}
