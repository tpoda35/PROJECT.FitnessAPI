using FitnessAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        //DbSets

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}
