using Microsoft.AspNetCore.Identity;
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
    
    //Tworzenie konta admina i przykładowych obiektów
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var hasher = new PasswordHasher<User>();
        var admin = new User
        {
            Id = 1,
            Email = "admin@admin.com",
            Role = "Admin"
        };
        
        // Hashowanie hasła admina
        admin.PasswordHash = hasher.HashPassword(admin, "admin");

        // Stworzenie admina
        modelBuilder.Entity<User>().HasData(admin);
        
        //Stworzenie przykładowych obiektów
        modelBuilder.Entity<Facility>().HasData(
            new Facility
            {
                Id = 1, 
                Name = "Conference Room A", 
                Description = "First Floor",
                Price = 100.0,
                Size = 50,
                Occupancy = "50 people",
                ImageUrl = "/images/conference_room_a.jpg"
            },
            new Facility 
            { 
                Id = 2, 
                Name = "Gymnasium", 
                Description = "Second Floor",
                Price = 150.0,
                Size = 200,
                Occupancy = "200 people",
                ImageUrl = "/images/gymnasium.jpg"
            },
            new Facility
            {
                Id = 3, 
                Name = "Auditorium", 
                Description = "Ground Floor",
                Price = 250.0,
                Size = 300,
                Occupancy = "300 people",
                ImageUrl = "/images/auditorium.jpg"
            }
        );
    }   
}