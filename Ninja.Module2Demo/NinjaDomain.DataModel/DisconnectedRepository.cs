using System;
using System.Collections.Generic;
using System.Linq;
using NinjaDomain.Classes;
using System.Collections;
using System.Data.Entity;

namespace NinjaDomain.DataModel
{
    public class DisconnectedRepository
    {
        public List<Ninja> GetNinjasWithClan()
        {
            using (var context = new NinjaContext())
            {
                return context
                            .Ninjas
                            .AsNoTracking()
                            .Include(n => n.Clan)
                            .ToList();
            }
        }

        public Ninja GetNinjaWithEquipment(int id)
        {
            using (var context = new NinjaContext())
            {
                return context
                            .Ninjas
                            .AsNoTracking()
                            .Include(n => n.EquipmentOwned)
                            .FirstOrDefault(n => n.Id == id);
            }
        }

        public Ninja GetNinjaWithEquipmentAndClan(int id)
        {
            using (var context = new NinjaContext())
            {
                return context
                            .Ninjas
                            .AsNoTracking()
                            .Include(n => n.EquipmentOwned)
                            .Include(n => n.Clan)
                            .FirstOrDefault(n => n.Id == id);
            }
        }

        public Ninja GetNinjaById(int id)
        {
            using (var context = new NinjaContext())
            {
                return context
                            .Ninjas
                            .AsNoTracking()
                            .SingleOrDefault(n => n.Id == id);
            }
        }

        public IEnumerable GetClans()
        {
            using (var context = new NinjaContext())
            {
                return context
                            .Clans
                            .AsNoTracking()
                            .OrderBy(c => c.ClanName)
                            .Select(c => new { c.Id, c.ClanName })
                            .ToList();
            }
        }

        public void SaveNewNinja(Ninja ninja)
        {
            using (var context = new NinjaContext())
            {
                context.Ninjas.Add(ninja);
                context.SaveChanges();
            }
        }

        public void SaveUpdatedNinja(Ninja ninja)
        {
            using (var context = new NinjaContext())
            {
                context.Entry(ninja).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public void DeleteNinja(int id)
        {
            using (var context = new NinjaContext())
            {
                var ninja = context.Ninjas.Find(id);
                context.Entry(ninja).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }

        public void SaveNewEquipment(NinjaEquipment equipment, int ninjaId)
        {
            using (var context = new NinjaContext())
            {
                // need to load Ninja and attach equipment as no FK in model
                // ...this could be avoided by adding a NinjaId to NinjaEquipemnt!
                var ninja = context.Ninjas.Find(ninjaId);
                ninja.EquipmentOwned.Add(equipment);
                context.SaveChanges();
            }
        }

        public void SaveUpdatedEquipment(NinjaEquipment equipment, int ninjaId)
        {
            using (var context = new NinjaContext())
            {
                // again, need to load existing entity as no FK in model and Ninja marked as [Required]
                // ...and again, this could be avoided by adding a NinjaId to NinjaEquipemnt!
                var equipmentWithNinjaFromDb = context
                                                    .Equipment
                                                    .Include(e => e.Ninja)
                                                    .FirstOrDefault(e => e.Id == equipment.Id);

                context
                    .Entry(equipmentWithNinjaFromDb)
                    .CurrentValues
                    .SetValues(equipment);

                context.SaveChanges();
            }
        }

        public List<Ninja> GetQueryableNinjasWithClan(string query, int page, int pageSize)
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;

                var linkQuery = context.Ninjas.Include(n => n.Clan);

                if (!string.IsNullOrEmpty(query))
                    linkQuery = linkQuery
                                    .Where(n => n.Name.Contains(query));

                if (page > 0 && pageSize > 0)
                    linkQuery = linkQuery
                                    .OrderBy(n => n.Name)
                                    .Skip(page - 1)
                                    .Take(pageSize);

                return linkQuery.ToList();
            }
        }
    }
}
