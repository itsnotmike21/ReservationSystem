using ReservationSystem.Models;

namespace ReservationSystem.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            // Jeśli nie ma użytkowników – dodaj admina
            if (!context.Users.Any())
            {
                var admin = new User
                {
                    Email = "admin@admin.com",
                    Role = "Admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin")
                };

                context.Users.Add(admin);
            }

            // Jeśli nie ma obiektów – dodaj przykładowe
            if (!context.Facilities.Any())
            {
                context.Facilities.AddRange(
                    new Facility
                    {
                        Name = "Conference Room A",
                        Description = "First Floor",
                        Price = 100.0,
                        Size = 50,
                        Occupancy = "50 people",
                        ImageUrl = "/images/conference_room_a.jpg"
                    },
                    new Facility
                    {
                        Name = "Gymnasium",
                        Description = "Second Floor",
                        Price = 150.0,
                        Size = 200,
                        Occupancy = "200 people",
                        ImageUrl = "/images/gymnasium.jpg"
                    }
                );
            }

            context.SaveChanges();
        }
    }
}