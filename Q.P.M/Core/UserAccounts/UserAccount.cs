using System;
using System.Collections.Generic;
using System.Text;
using static Q.P.M.Modules.Methods.ItemMethod;

namespace Q.P.M.Core.UserAccounts
{
    public class UserAccount
    {
        public ulong Id { get; set; }

        public string Name { get; set; }

        public string test { get; set; }

        public int PageNumber { get; set; } = 1;

        public CharacterSheet CurrentCharacter { get; set; } = new CharacterSheet();

        public CharacterSheet Character1 { get; set; } = new CharacterSheet();

        public CharacterSheet Character2 { get; set; } = new CharacterSheet();

        public CharacterSheet Character3 { get; set; } = new CharacterSheet();
    }

    public class CharacterSheet
    {
        public ulong CurentCharacter { get; set; } = 0; //1 2 or 3

        public bool IsMade { get; set; } = false;

        public string Name { get; set; } = null;

        public string Gender { get; set; } = null;

        public string ExoClass { get; set; } = null;

        public string EquipedWeapon { get; set; } = null;

        public string EquipedShield { get; set; } = null;

        public uint Xp { get; set; } = 0;

        public uint Level => (uint)Math.Sqrt(Xp / 12);

        public List<Slot> Inventory { get; set; } = new List<Slot>();

        public uint Maxhealth { get; set; } = 0;

        public uint CurrentHealth { get; set; } = 0;

        public ulong Gold { get; set; } = 200;

        public ulong PvPFightsWon { get; set; } = 0;

        public ulong ExtraDamage { get; set; } = 0;

        public ulong ExtraShield { get; set; } = 0;
    }
}
