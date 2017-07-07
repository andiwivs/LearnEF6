using System;
using System.Collections.Generic;
using System.Data;
using NinjaDomain.DataModel;
using static NinjaDomain.Classes;
using System.Data.Entity;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            SkipDatabaseInitialisation();
            //InsertNinja();
            //InsertMultipleNinjas();
            //SimpleNinjaQuery();
            //SimpleNinjaGraphQuery();
            //QueryAndUpdateNinja();
            //QueryAndUpdateNinjaDisconnected();

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

        private static void SimpleNinjaQuery()
        {
            throw new NotImplementedException();
        }

        private static void SimpleNinjaGraphQuery()
        {
            throw new NotImplementedException();
        }

        private static void QueryAndUpdateNinja()
        {
            throw new NotImplementedException();
        }

        private static void QueryAndUpdateNinjaDisconnected()
        {
            throw new NotImplementedException();
        }
    }
}
