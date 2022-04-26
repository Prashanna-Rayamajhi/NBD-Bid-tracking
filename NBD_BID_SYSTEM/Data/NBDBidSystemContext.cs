using Microsoft.EntityFrameworkCore;
using NBD_BID_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBD_BID_SYSTEM.Data
{
    public class NBDBidSystemContext : DbContext
    {
        public NBDBidSystemContext(DbContextOptions<NBDBidSystemContext> options)
        : base(options)
        {
        }

        public DbSet<Client> Clients  { get; set; }
        public DbSet<Project> Projects { get; set; }

        public DbSet<Province> Provinces { get; set; }

        public DbSet<Bid> Bids { get; set; }

        public DbSet<BidLabor> BidLabors { get; set; }

        public DbSet<Labor> Labors { get; set; }

        public DbSet<Material> Materials { get; set; }

        public DbSet<Inventory> Inventories { get; set; }

        public DbSet<InventoryType> InventoryTypes { get; set; }
        public DbSet<Staff> Staffs { get; set; }

        public DbSet<Position> Positions { get; set; }

        public DbSet<BidStaff> BidStaffs { get; set; }

        public DbSet<ApproveBid> ApproveBids { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bid>()
                .HasOne(b => b.ApproveBid)
                .WithMany(b => b.Bids)
                .HasForeignKey(b => b.ApproveBidID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BidStaff>()
                .HasKey(b => new { b.BidID, b.StaffID });

            modelBuilder.Entity<BidStaff>()
                .HasOne(b => b.Bid)
                .WithMany(b => b.BidStaffs)
                .HasForeignKey(b => b.BidID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Client>()
                .HasMany<Project>(c => c.Projects)
                .WithOne(p => p.Client)
                .HasForeignKey(c=>c.ClientID)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent Cascade Delete from Client to Project
            modelBuilder.Entity<Project>()
                .HasMany(c => c.Bids)
                .WithOne(b => b.Project)
                .HasForeignKey(b => b.ProjectID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventoryType>()
                .HasMany(t => t.Inventories)
                .WithOne(i => i.InventoryType)
                .HasForeignKey(i => i.InventoryTypeID)
                .OnDelete(DeleteBehavior.Restrict);

 

            modelBuilder.Entity<Position>()
                .HasMany(p => p.Staffs)
                .WithOne(s => s.Position)
                .HasForeignKey(s => s.PositionID)
                .OnDelete(DeleteBehavior.Restrict);

            //Creating the unique key for code
            modelBuilder.Entity<Inventory>()
                .HasIndex(c => c.Code)
                .IsUnique();

	//Add a unique index to the Employee Email
            modelBuilder.Entity<Staff>()
            .HasIndex(a => new { a.Email})
            .IsUnique();



        }

    }
}
