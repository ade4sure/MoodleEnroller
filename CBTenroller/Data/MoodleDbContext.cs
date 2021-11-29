using CBTenroller.Models;
using Microsoft.EntityFrameworkCore;

namespace CBTenroller.Data
{
    public class MoodleDbContext : DbContext
    {
        public MoodleDbContext(DbContextOptions<MoodleDbContext> options)
            : base(options)
        {
        }
        public DbSet<Quiz_Attempts> Quiz_Attempts { get; set; }

    }
}

