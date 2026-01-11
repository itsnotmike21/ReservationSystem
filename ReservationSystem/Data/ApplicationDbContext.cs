using Microsoft.EntityFrameworkCore;
using ReservationSystem.Models;

namespace ReservationSystem.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Facility> Facilities { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
}