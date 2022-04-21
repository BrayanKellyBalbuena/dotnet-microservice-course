using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data.Db
{
    public class AppDbContext : DbContext 
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) :  base(opt)
        {

        }

        public DbSet<Platform> Platforms => Set<Platform>();
    }

}