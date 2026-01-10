namespace ReservationSystem.Models;

public class Reservation
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }
    
    public int FacilityId { get; set; }
    public Facility Facility { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public string? Notes { get; set; }
}