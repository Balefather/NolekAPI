﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NolekAPI.Model;

namespace NolekAPI.Data
{
    public class NolekAPIContext : DbContext
    {
        public NolekAPIContext (DbContextOptions<NolekAPIContext> options)
            : base(options)
        {
        }

        public DbSet<NolekAPI.Model.Part> tblParts { get; set; } = default!;

        public DbSet<NolekAPI.Model.Service> tblServices { get; set; }
        public DbSet<ServiceView> vw_Services { get; set; }
        public DbSet<NolekAPI.Model.CustomersView>? vw_CustomersMachinesParts { get; set; }

        public DbSet<NolekAPI.Model.MachineView>? vw_MachineParts { get; set; }

        public DbSet<NolekAPI.Model.ServicePartJunction>? tblServices_Parts { get; set; }

        public DbSet<Invoice> Invoice { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServiceView>().ToView("vw_Services");
            modelBuilder.Entity<ServiceView>().HasNoKey().ToView("vw_Services");

            modelBuilder.Entity<MachineView>().ToView("vw_MachineParts");
            modelBuilder.Entity<MachineView>().HasNoKey().ToView("vw_MachineParts");


            modelBuilder.Entity<CustomersView>().ToView("vw_CustomersMachinesParts");
            modelBuilder.Entity<CustomersView>().HasNoKey().ToView("vw_CustomersMachinesParts");
            //modelBuilder.Entity<UserRole>().HasNoKey().ToView("tblUserRoles");
            modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserID, ur.RoleID });
            modelBuilder.Entity<ServicePartJunction>().HasKey(sp => new { sp.PartID, sp.ServiceID });
            //modelBuilder.Entity<Service>().HasKey(sp => new { sp.ServiceID });
            //modelBuilder.Entity<Customer>().ToView("vw_CustomersMachinesParts");
            modelBuilder.Entity<Invoice>().HasNoKey().ToView(null);
        }

        public DbSet<NolekAPI.Model.User>? tblUsers { get; set; }
        public DbSet<NolekAPI.Model.UserRole>? tblUserRoles { get; set; }
        public DbSet<NolekAPI.Model.Role>? tblRoles { get; set; }

    }
}
