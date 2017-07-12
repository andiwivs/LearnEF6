using System;
using System.Collections.Generic;
using NinjaDomain.Classes.Interfaces;

namespace NinjaDomain.Classes
{
    public class Ninja : IModificationHistory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool ServedInOniwaban { get; set; }
        public Clan Clan { get; set; }
        public int ClanId { get; set; }
        public List<NinjaEquipment> EquipmentOwned { get; set; } // mark as virtual to enable lazy-loading of relational data
        public DateTime DateOfBirth { get; set; }

        public DateTime DateModified { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsDirty { get; set; }

        public Ninja()
        {
            EquipmentOwned = new List<NinjaEquipment>();
        }
    }
}
