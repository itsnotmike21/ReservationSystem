namespace ReservationSystem.Models;

public class Reservation
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public int PeopleCount { get; set; }
    public string Notes { get; set; }
}