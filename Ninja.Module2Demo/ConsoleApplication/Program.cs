using System;
using System.Collections.Generic;
using System.Data;
using NinjaDomain.DataModel;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using NinjaDomain;
using NinjaDomain.Classes;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            SkipDatabaseInitialisation();

            //InsertNinja();
            //InsertMultipleNinjas();
            //InsertNinjaWithEquipment();

            //SimpleNinjaQuery();
            //SimpleNinjaGraphQuery();

            //RetrieveDataWithFind();
            //RetrieveDataWithStoredProc();

            //QueryAndUpdateNinja();
            //QueryAndUpdateNinjaDisconnected();

            //DeleteNinja();
            //DeleteNinjaDisconnected();
            //DeleteNinjaById();

            Console.ReadKey();
        }

        private static void SkipDatabaseInitialisation()
        {
            Database.SetInitializer(new NullDatabaseInitializer<NinjaContext>());
        }

        private static void InsertNinja()
        {
            var ninja = new Ninja
            {
                Name = "Anders-san1",
                ServedInOniwaban = false,
                DateOfBirth = new DateTime(1977, 11, 22),
                ClanId = 1
            };

            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                context.Ninjas.Add(ninja);
                context.SaveChanges();
            }
        }

        private static void InsertMultipleNinjas()
        {
            var ninja1 = new Ninja
            {
                Name = "Leonardo",
                ServedInOniwaban = false,
                DateOfBirth = new DateTime(1984, 1, 1),
                ClanId = 1
            };

            var ninja2 = new Ninja
            {
                Name = "Raphael",
                ServedInOniwaban = false,
                DateOfBirth = new DateTime(1985, 1, 1),
                ClanId = 1
            };

            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                context.Ninjas.AddRange(new List<Ninja>() { ninja1, ninja2 });
                context.SaveChanges();
            }
        }

        private static void InsertNinjaWithEquipment()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.Write;

                var ninja = new Ninja
                {
                    Name = "Important Ninja",
                    ServedInOniwaban = false,
                    ClanId = 1,
                    DateOfBirth = new DateTime(2000, 1, 1)
                };

                var muscles = new NinjaEquipment
                {
                    Name = "Muscle",
                    Type = EquipmentType.Tool
                };

                var stamina = new NinjaEquipment
                {
                    Name = "Stamina",
                    Type = EquipmentType.Tool
                };

                context.Ninjas.Add(ninja);
                ninja.EquipmentOwned.AddRange(new List<NinjaEquipment> { muscles, stamina }); // note: equipment will automatically be tracked by context
                context.SaveChanges();
            }
        }

        private static void SimpleNinjaQuery()
        {
            using (var context = new NinjaContext())
            {

                // note: using enumerable means db connection open for whole foreach loop...
                var query = context.Ninjas;

                foreach (var ninja in query)
                {
                    Console.WriteLine(ninja.Name);
                }

                // better: execute query and then loop through the items...
                var allNinjas = context.Ninjas.ToList();

                foreach (var ninja in allNinjas)
                {
                    Console.WriteLine(ninja.Name);
                }

                // eg: filtering
                var pagedResult = context
                    .Ninjas
                    .Where(n => n.DateOfBirth >= new DateTime(1977, 1, 1))
                    .OrderBy(n => n.Name)
                    .Skip(1)
                    .Take(2)
                    .ToList();

                Console.WriteLine(string.Join(",", pagedResult.Select(n => n.Name).ToArray()));
            }
        }

        private static void SimpleNinjaGraphQuery()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.Write;

                // option 1: eager loading - using include can be inefficient as includes build up
                //var ninja1 = context
                //                .Ninjas
                //                .Include(n => n.EquipmentOwned)
                //                .FirstOrDefault(n => n.Name.StartsWith("important"));

                // option 2: explicit loading - load only what you need
                //var ninja2 = context
                //                .Ninjas
                //                .FirstOrDefault(n => n.Name.StartsWith("important"));

                //context
                //    .Entry(ninja2)
                //    .Collection(n => n.EquipmentOwned)
                //    .Load();

                // option 3: lazy loading - can be inefficient
                //var ninja3 = context
                //                .Ninjas
                //                .FirstOrDefault(n => n.Name.StartsWith("important"));

                //int equipmentCount = ninja3.EquipmentOwned.Count; // with property marked as virtual, we can trigger lazy-load behaviour

                // option 4: projection queries
                var ninjas = context
                                .Ninjas
                                .Select(n => new { n.Name, n.DateOfBirth, n.EquipmentOwned })  // note: equipment owned collection will be populated for every item
                                .ToList();
            }
        }

        private static void RetrieveDataWithFind()
        {

            var keyval = 4;

            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;

                // note: log will show we query db by id
                var ninja1 = context.Ninjas.Find(keyval);
                Console.WriteLine($"After find#1 : {ninja1.Name}");

                // note: log will show we skip db query as item already loaded into context
                var ninja2 = context.Ninjas.Find(keyval);
                Console.WriteLine($"After find#1 : {ninja2.Name}");
            }
        }

        private static void RetrieveDataWithStoredProc()
        {

            // create procedure GetOldNinjas as select * from Ninjas where dateofbirth < '1/1/1980'

            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;

                var ninjas = context
                    .Ninjas
                    .SqlQuery("exec GetOldNinjas")
                    .ToList();

                foreach (var ninja in ninjas)
                {
                    Console.WriteLine(ninja.Name);
                }
            }
        }

        private static void QueryAndUpdateNinja()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;

                var ninja = context.Ninjas.First();

                ninja.ServedInOniwaban = !ninja.ServedInOniwaban;
                context.SaveChanges();
            }
        }

        private static void QueryAndUpdateNinjaDisconnected()
        {

            Ninja ninja;

            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                ninja = context.Ninjas.First();
            }

            // change is made outside of a using/connection
            ninja.ServedInOniwaban = !ninja.ServedInOniwaban;

            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;

                // note: need to attach and flag as modified as we changed the ninja outside of this context
                context.Ninjas.Attach(ninja);
                context.Entry(ninja).State = EntityState.Modified;

                context.SaveChanges();
            }
        }

        private static void DeleteNinja()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;

                // create test data...
                context.Ninjas.Add(new Ninja
                {
                    Name = "TEMP Ninja",
                    ClanId = 1,
                    DateOfBirth = new DateTime(2017, 7, 11)
                });

                context.SaveChanges();

                // ...and then delete it
                var ninja = context
                    .Ninjas
                    .OrderByDescending(n => n.Id)
                    .First();

                context.Ninjas.Remove(ninja);
                context.SaveChanges();
            }
        }

        private static void DeleteNinjaDisconnected()
        {

            Ninja tempNinja;

            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;

                // create test data...
                context.Ninjas.Add(new Ninja
                {
                    Name = "TEMP Ninja",
                    ClanId = 1,
                    DateOfBirth = new DateTime(2017, 7, 11)
                });

                context.SaveChanges();

                // load using first conext
                tempNinja = context
                    .Ninjas
                    .OrderByDescending(n => n.Id)
                    .First();
            }

            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;

                // to delete using different context, need to attach with flag...
                context.Entry(tempNinja).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }

        private static void DeleteNinjaById()
        {
            int id;

            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;

                // create test data...
                context.Ninjas.Add(new Ninja
                {
                    Name = "TEMP Ninja",
                    ClanId = 1,
                    DateOfBirth = new DateTime(2017, 7, 11)
                });

                context.SaveChanges();

                // ...and extract record id
                id = context
                        .Ninjas
                        .OrderByDescending(n => n.Id)
                        .First()
                        .Id;
            }

            // delete by id
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;

                var ninja = context.Ninjas.Find(id); // first sql operation
                context.Ninjas.Remove(ninja);
                context.SaveChanges(); // second sql operation
            }

            /*
            // optimisation: delete by procedure = single sql operation
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                context.Database.ExecuteSqlCommand("exec DeleteNinjaById {0}", id);
            }
            */
        }
    }
}
