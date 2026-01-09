namespace ReservationSystem.Models;

public class Facility
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public int Size {get; set;}
    public string Occupancy {get; set;}
    public string ImageUrl { get; set; } // Ścieżka do obrazu reprezentującego obiekt
}
