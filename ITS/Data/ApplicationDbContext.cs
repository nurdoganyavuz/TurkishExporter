using KobiAsITS.Models;
using Microsoft.EntityFrameworkCore;

namespace KobiAsITS.Data
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Process> Processes { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestFile> RequestFiles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
