using CBTenroller.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CBTenroller.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<MoodleUser> MoodleUsers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<CourseHall> CourseHalls { get; set; }
        public DbSet<CourseEnrollment> CourseEnrollments { get; set; }
        public DbSet<Fingers> Fingers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<CourseHall>()
                .HasAlternateKey(a => new { a.CourseId, a.HallId });  // create unique composite key

            builder.Entity<CourseEnrollment>()
                .HasAlternateKey(a => new { a.MoodleUserId, a.CourseId });  // create unique composite key

        }
    }
}

