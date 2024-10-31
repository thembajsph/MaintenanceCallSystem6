//using MaintenanceCallSystem6.Models;
//using Microsoft.EntityFrameworkCore;

//namespace MaintenanceCallSystem6.Data;

//public class ApplicationDbContext : DbContext
//{
//    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
//        : base(options) { }

//    public DbSet<User> Users { get; set; }
//    public DbSet<MaintenanceCall> MaintenanceCalls { get; set; }
//}

using MaintenanceCallSystem6.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MaintenanceCallSystem6.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<MaintenanceCall> MaintenanceCalls { get; set; }

        // Add the DbSet for Technicians
        public DbSet<Technician> Technicians { get; set; }
    }
}
