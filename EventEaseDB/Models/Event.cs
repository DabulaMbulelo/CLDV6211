namespace EventEaseDB.Models
{
    public class Event
    {
        public string EventId { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string Description { get; set; }
        public int VenueId { get; set; }
    }
}
