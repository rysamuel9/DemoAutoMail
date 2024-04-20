using DemoAutoMail.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAutoMail.Data
{
    public class AutomailDbContext : DbContext
    {
        public AutomailDbContext(DbContextOptions<AutomailDbContext> options)
           : base(options)
        {
        }

        public DbSet<Automail> Automail { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Automail>(entity =>
            //{
            //    entity.Property(e => e.Subject).IsRequired();
            //    entity.Property(e => e.Body).IsRequired();
            //    entity.Property(e => e.Sender).IsRequired();
            //    entity.Property(e => e.EmailTo).IsRequired();
            //    entity.Property(e => e.IsSend).HasDefaultValue(false);
            //    entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            //});
        }
    }
}
