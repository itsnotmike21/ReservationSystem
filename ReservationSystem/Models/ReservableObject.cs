namespace ReservationSystem.Models;

public class ReservableObject
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public string Description { get; set; }
    public string ImagePath { get; set; } // Ścieżka do obrazu reprezentującego obiekt
}
