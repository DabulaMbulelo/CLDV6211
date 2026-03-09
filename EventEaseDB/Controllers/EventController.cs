using EventEaseDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EventEaseDB.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDBContext _context;
        public EventController(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var Events = await _context.Event.Include(e => e.Venue).ToListAsync();
            return View(Events);
        }
        public IActionResult Create()
        {
            ViewData["Venues"] = _context.Venue.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event Events)
        {
            if (ModelState.IsValid)
            {
                _context.Event.Add(Events);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(Events);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Events = await _context.Event
                .Include(e => e.Venue) // Include related venue if applicable
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (Events == null)
            {
                return NotFound();
            }

            return View(Events);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var Events = await _context.Event.Include(e => e.Venue).FirstOrDefaultAsync(m => m.EventId == id);

            if (Events == null) return NotFound();

            var isBooked = await _context.Booking.AnyAsync(b => b.EventId == id);
            ViewData["IsBooked"] = isBooked;

            return View(Events);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var Events = await _context.Event.FindAsync(id);
            if (Events == null) return NotFound();

            var isBooked = await _context.Booking.AnyAsync(b => b.EventId == id);
            if (isBooked)
            {
                ModelState.AddModelError("", "Cannot delete this event because it has associated bookings.");
                var reloaded = await _context.Event.Include(e => e.Venue).FirstOrDefaultAsync(m => m.EventId == id);
                ViewData["IsBooked"] = true;
                return View("Delete", reloaded ?? Events);
            }
            _context.Event.Remove(Events);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public bool EventExists(int id)
        {
            return _context.Event.Any(e => e.EventId == id);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var Events = await _context.Event.FindAsync(id);
            if (Events == null) return NotFound();
            ViewBag.VenueId = new SelectList(_context.Venue.ToList(), "VenueId", "VenueName", Events.VenueId);
            ViewData["Venues"] = _context.Venue.ToList();
            return View(Events);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event Events)
        {
            if (id != Events.EventId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Events);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(Events.EventId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.VenueId = new SelectList(_context.Venue.ToList(), "VenueId", "VenueName", Events.VenueId);
            ViewData["Venues"] = _context.Venue.ToList();
            return View(Events);
        }
    }
}
