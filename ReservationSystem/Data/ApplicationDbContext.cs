using Microsoft.EntityFrameworkCore;
using ReservationSystem.Models;

namespace ReservationSystem.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Reservation> Reservations { get; set; }
}