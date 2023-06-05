using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NolekAPI.Model;
using NolekAPI.Model.Dto;
using NolekAPI.Model.Dto.Junction;
using NolekAPI.Model.View;

namespace NolekAPI.Data
{
    public class NolekAPIContext : DbContext
    {
        public NolekAPIContext (DbContextOptions<NolekAPIContext> options)
            : base(options)
        {
        }
        //tables
        public DbSet<PartDto> tblParts { get; set; } = default!;
        public DbSet<MachineDto> tblMachines { get; set; }
        public DbSet<MachineSerialNumbersDto> tblMachineSerialNumbers { get; set; }
        public DbSet<ImageDto> tblImages { get; set; }

        public DbSet<ServiceImageJunctionDto> tblServices_Images { get; set; }

        public DbSet<MachinePartJunctionDto> tblMachines_Parts{ get; set; }

        public DbSet<CustomerMachineJunctionDto> tblCustomers_Machines { get; set; }

        public DbSet<ServiceDto> tblServices { get; set; }

        public DbSet<UserDto>? tblUsers { get; set; }

        public DbSet<RoleDto>? tblRoles { get; set; }

        public DbSet<UserRoleJunctionDto>? tblUserRoles { get; set; }

        public DbSet<ServicePartJunctionDto>? tblServices_Parts { get; set; }

        //Views
        public DbSet<ServiceView> vw_Services { get; set; }
        public DbSet<CustomersView>? vw_CustomersMachinesParts { get; set; }

        public DbSet<MachineView>? vw_MachineParts { get; set; }



        //what

        public DbSet<Invoice> Invoice { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //tables

            //junctions
            modelBuilder.Entity<UserRoleJunctionDto>().HasKey(ur => new { ur.UserID, ur.RoleID });
            modelBuilder.Entity<ServicePartJunctionDto>().HasKey(sp => new { sp.PartID, sp.ServiceID });
            modelBuilder.Entity<ServiceImageJunctionDto>().HasKey(si => new { si.ImageID, si.ServiceID });
            modelBuilder.Entity<MachinePartJunctionDto>().HasKey(si => new { si.MachineID});

            //views
            modelBuilder.Entity<ServiceView>().HasNoKey().ToView("vw_Services");
            modelBuilder.Entity<MachineView>().HasNoKey().ToView("vw_MachineParts");
            modelBuilder.Entity<CustomersView>().HasNoKey().ToView("vw_CustomersMachinesParts");


            //mysteries
            modelBuilder.Entity<Invoice>().HasNoKey().ToView(null);
        }



    }
}
