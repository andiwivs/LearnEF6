using System;
using NinjaDomain.Classes;
using System.Data.Entity;
using System.Linq;
using NinjaDomain.Classes.Interfaces;

namespace NinjaDomain.DataModel
{
    public class NinjaContext : DbContext
    {
        public DbSet<Ninja> Ninjas { get; set; }
        public DbSet<Clan> Clans { get; set; }
        public DbSet<NinjaEquipment> Equipment { get; set; }

        public NinjaContext() : base("NinjaDomainConnectionString") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // do not track IsDirty property in data model
            modelBuilder
                .Types()
                .Configure(t => t.Ignore("IsDirty"));

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            // performing sweeping update to auto-maintain created/modified dates for entities implementing our IModificationHistory interface...
            ChangeTracker
                .Entries()
                .Where(e => e.Entity is IModificationHistory)
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity as IModificationHistory)
                .ToList()
                .ForEach(h =>
                {
                    h.DateModified = DateTime.Now;

                    if (h.DateCreated == DateTime.MinValue)
                        h.DateCreated = DateTime.Now;
                });

            int result = base.SaveChanges();

            // for local model (and not persisted in data model as configured in OnModelCreating above) reset IsDirty flags
            ChangeTracker
                .Entries()
                .Where(e => e.Entity is IModificationHistory)
                .Select(e => e.Entity as IModificationHistory)
                .ToList()
                .ForEach(h => h.IsDirty = false);

            return result;
        }
    }
}
