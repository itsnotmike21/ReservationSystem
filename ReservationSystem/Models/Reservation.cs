using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ReservationSystem.Models;

public class Reservation
{
    public int Id { get; set; }
    
    public int UserId { get; set; }

    [ValidateNever]   // ← ASP.NET nie waliduje User
    public User User { get; set; }
    
    public int FacilityId { get; set; }

    [ValidateNever]   // ← ASP.NET nie waliduje Facility
    public Facility Facility { get; set; }
    
    [Display(Name = "Początek")]
    public DateTime StartTime { get; set; }
    [Display(Name = "Koniec")]
    public DateTime EndTime { get; set; }
    [Display(Name = "Notatki")]
    public string? Notes { get; set; }
}