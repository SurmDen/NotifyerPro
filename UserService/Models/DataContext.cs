using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using IdentityManager.Infrastructure;
using IdentityManager.Models;

namespace UserService.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().HasData(
                new User[]
                {
                    new User()
                    {
                        Id = 1,
                        Name = "Denis",
                        Email = "surmanidzedenis609@gmail.com",
                        Address = "Spb, School str.",
                        Telephone = "89958899265",
                        Role = "Admin",
                        Password = PasswordHesher.HeshPassword("12345$")
                    }
                }
                );
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Dialog> Dialogs { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Friend> Friends { get; set; }

        public DbSet<Like> Likes { get; set; }

    }
}
