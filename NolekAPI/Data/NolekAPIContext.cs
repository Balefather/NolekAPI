using System;
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

        public DbSet<NolekAPI.Model.CustomersMachinesParts>? vw_CustomersMachinesParts { get; set; }

        public DbSet<NolekAPI.Model.MachineParts>? vw_MachineParts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomersMachinesParts>().ToView("vw_MachineParts");
            modelBuilder.Entity<CustomersMachinesParts>().HasNoKey().ToView("vw_MachineParts");

            modelBuilder.Entity<CustomersMachinesParts>().ToView("vw_CustomersMachinesParts");
            modelBuilder.Entity<CustomersMachinesParts>().HasNoKey().ToView("vw_CustomersMachinesParts");
            //modelBuilder.Entity<Customer>().ToView("vw_CustomersMachinesParts");
        }

    }
}
