using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace ZA_PLACE.Models
{
    public class ApplicationDbContext : IdentityDbContext<UserExtra>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
        {
        }

        public DbSet<Age> Ages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<EnrollementCourse> EnrollementCourses { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seeding roles
            var adminRoleId = Guid.NewGuid().ToString();
            var studentRoleId = Guid.NewGuid().ToString();
            var teacherRoleId = Guid.NewGuid().ToString();

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = adminRoleId, Name = "ADMIN", NormalizedName = "ADMIN" },
                new IdentityRole { Id = studentRoleId, Name = "STUDENT", NormalizedName = "STUDENT" },
                new IdentityRole { Id = teacherRoleId, Name = "TEACHER", NormalizedName = "TEACHER" }
            );

            // Seeding admin user
            var userId = Guid.NewGuid().ToString(); // Use string for the IdentityUser Id
            var hasher = new PasswordHasher<UserExtra>();

            modelBuilder.Entity<UserExtra>().HasData(
                new UserExtra
                {
                    Id = userId, 
                    UserName = "admin@domain.com",
                    NormalizedUserName = "ADMIN@DOMAIN.COM",
                    Email = "admin@domain.com",
                    NormalizedEmail = "ADMIN@DOMAIN.COM",
                    EmailConfirmed = true,
                    PhoneNumber = "1234567890",
                    PhoneNumberConfirmed = true,
                    FullName = "Admin User",
                    Gender = true,
                    UserStatus = true,
                    DoB = new DateTime(1990, 1, 1),
                    CreatedOn = DateTime.Now,
                    PasswordHash = hasher.HashPassword(null, "Admin@1234"),
                    Permision = 0
                }
            );

            // Assign the admin user to the ADMIN role
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    UserId = userId, // Use the same UserId that was seeded
                    RoleId = adminRoleId
                }
            );
        }
    }
}
