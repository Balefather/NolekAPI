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

        public DbSet<NolekAPI.Model.tblParts> tblParts { get; set; } = default!;

        public DbSet<NolekAPI.Model.tblServices> tblServices { get; set; }
    }
}
