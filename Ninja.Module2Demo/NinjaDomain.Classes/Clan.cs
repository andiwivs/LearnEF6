using System;
using System.Collections.Generic;
using NinjaDomain.Classes.Interfaces;

namespace NinjaDomain.Classes
{
    public class Clan : IModificationHistory
    {
        public int Id { get; set; }
        public string ClanName { get; set; }
        public List<Ninja> Ninjas { get; set; }

        public DateTime DateModified { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsDirty { get; set; }

        public Clan()
        {
            Ninjas = new List<Ninja>();
        }
    }
}
