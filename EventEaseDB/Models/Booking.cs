using System.ComponentModel.DataAnnotations;

namespace EventEaseDB.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        [Required]
        public int EventId { get; set; }

        public Event? Event { get; set; }

        [Required]
        public int VenueId { get; set; }
        public Venue? Venue { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.Now;
    }
}
