using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TheLifeTimeTalents.Models;

namespace TheLifeTimeTalents.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<Talent> Talent { get; set; }//DbSet property used by all team members

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   //Reference: http://benjii.me/2016/05/dotnet-ef-migrations-for-asp-net-core/  
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                //entity.Relational().TableName = entity.DisplayName();
                entity.SetTableName(entity.DisplayName());
            }

            //Setup one-to-one relationship between User and Role entity type.   
            modelBuilder.Entity<User>()
                .HasOne(input => input.Role)
                .WithMany()
                .HasForeignKey(input => input.RoleID);
            base.OnModelCreating(modelBuilder);
        }
    }
}
