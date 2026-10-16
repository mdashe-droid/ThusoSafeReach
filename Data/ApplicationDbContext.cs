using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ThusoSafeReach.Models;

namespace ThusoSafeReach.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<CheckIn> CheckIns => Set<CheckIn>();
        public DbSet<SOSAlert> SOSAlerts => Set<SOSAlert>();
        public DbSet<Incident> Incidents => Set<Incident>();
        public DbSet<MedicalProfile> MedicalProfiles => Set<MedicalProfile>();
        public DbSet<EmergencyContact> EmergencyContacts => Set<EmergencyContact>();
    }
}
