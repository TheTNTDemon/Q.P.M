using System;
using System.Collections.Generic;
using System.Text;

namespace Q.P.M.Addons
{
    class ItemDictionary
    {
        public Dictionary<int, Item> Items { get; set; } = new Dictionary<int, Item>();
    }

    public class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Class { get; set; }
        public int Damage { get; set; } = 0;
        public int Protection { get; set; } = 0;
        public bool Stackable { get; set; }
    }
}
