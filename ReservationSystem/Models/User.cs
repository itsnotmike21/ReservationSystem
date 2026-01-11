using System.ComponentModel.DataAnnotations;
namespace ReservationSystem.Models;

public class User
{
    [Display(Name = "ID użytkownika")]
    public int Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; } // "User" lub "Admin"
}
